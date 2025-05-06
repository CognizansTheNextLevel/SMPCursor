using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using ClientTrackPro.Core.Models.Account;
using ClientTrackPro.Core.Interfaces.Services;
using ClientTrackPro.Core.Configuration;

namespace ClientTrackPro.Core.Services
{
    public class PayPalService : IPayPalService
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _appSettings;
        private string _accessToken;

        public PayPalService(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_appSettings.PayPal.UseSandbox
                    ? "https://api-m.sandbox.paypal.com"
                    : "https://api-m.paypal.com")
            };
        }

        public async Task<string> CreateSubscriptionAsync(SubscriptionTier tier, bool isAnnual)
        {
            await EnsureAccessTokenAsync();

            var price = await GetSubscriptionPriceAsync(tier, isAnnual);
            var planId = await CreateBillingPlanAsync(tier, isAnnual, price);
            var subscription = await CreateSubscriptionAsync(planId);

            return subscription;
        }

        public async Task<bool> ProcessPaymentAsync(string paymentId)
        {
            await EnsureAccessTokenAsync();

            var response = await _httpClient.GetAsync($"/v1/payments/payment/{paymentId}");
            if (!response.IsSuccessStatusCode)
                return false;

            var content = await response.Content.ReadAsStringAsync();
            var payment = JsonSerializer.Deserialize<PayPalPayment>(content);

            return payment.State == "approved";
        }

        public async Task<bool> CancelSubscriptionAsync(string subscriptionId)
        {
            await EnsureAccessTokenAsync();

            var response = await _httpClient.PostAsync(
                $"/v1/billing/subscriptions/{subscriptionId}/cancel",
                new StringContent(""));

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ValidatePaymentAsync(string paymentId)
        {
            await EnsureAccessTokenAsync();

            var response = await _httpClient.GetAsync($"/v1/payments/payment/{paymentId}");
            if (!response.IsSuccessStatusCode)
                return false;

            var content = await response.Content.ReadAsStringAsync();
            var payment = JsonSerializer.Deserialize<PayPalPayment>(content);

            return payment.State == "approved" && 
                   payment.Payer.Status == "VERIFIED";
        }

        public async Task<decimal> GetSubscriptionPriceAsync(SubscriptionTier tier, bool isAnnual)
        {
            return tier switch
            {
                SubscriptionTier.Streamer => isAnnual ? 99.99m : 9.99m,
                SubscriptionTier.Mogul => isAnnual ? 299.99m : 29.99m,
                _ => throw new ArgumentException("Invalid subscription tier")
            };
        }

        public async Task<bool> RefundPaymentAsync(string paymentId, decimal amount)
        {
            await EnsureAccessTokenAsync();

            var refund = new
            {
                amount = new
                {
                    total = amount.ToString("F2"),
                    currency = "USD"
                }
            };

            var response = await _httpClient.PostAsync(
                $"/v1/payments/sale/{paymentId}/refund",
                new StringContent(JsonSerializer.Serialize(refund)));

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateSubscriptionAsync(string subscriptionId, SubscriptionTier newTier)
        {
            await EnsureAccessTokenAsync();

            var price = await GetSubscriptionPriceAsync(newTier, true);
            var planId = await CreateBillingPlanAsync(newTier, true, price);

            var response = await _httpClient.PostAsync(
                $"/v1/billing/subscriptions/{subscriptionId}/revise",
                new StringContent(JsonSerializer.Serialize(new { plan_id = planId })));

            return response.IsSuccessStatusCode;
        }

        private async Task EnsureAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken))
                return;

            var auth = Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(
                    $"{_appSettings.PayPal.ClientId}:{_appSettings.PayPal.ClientSecret}"));

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);

            var response = await _httpClient.PostAsync(
                "/v1/oauth2/token",
                new StringContent("grant_type=client_credentials"));

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to obtain PayPal access token");

            var content = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<PayPalToken>(content);

            _accessToken = token.AccessToken;
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
        }

        private async Task<string> CreateBillingPlanAsync(SubscriptionTier tier, bool isAnnual, decimal price)
        {
            var plan = new
            {
                name = $"{tier} Plan ({(isAnnual ? "Annual" : "Monthly")})",
                description = $"Subscription to the {tier} tier",
                type = "FIXED",
                payment_definitions = new[]
                {
                    new
                    {
                        name = "Regular Payment",
                        type = "REGULAR",
                        frequency = isAnnual ? "YEAR" : "MONTH",
                        frequency_interval = "1",
                        amount = new
                        {
                            value = price.ToString("F2"),
                            currency = "USD"
                        },
                        cycles = "0"
                    }
                }
            };

            var response = await _httpClient.PostAsync(
                "/v1/payments/billing-plans",
                new StringContent(JsonSerializer.Serialize(plan)));

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to create billing plan");

            var content = await response.Content.ReadAsStringAsync();
            var createdPlan = JsonSerializer.Deserialize<PayPalPlan>(content);

            // Activate the plan
            await _httpClient.PatchAsync(
                $"/v1/payments/billing-plans/{createdPlan.Id}",
                new StringContent(JsonSerializer.Serialize(new[]
                {
                    new
                    {
                        op = "replace",
                        path = "/",
                        value = new { state = "ACTIVE" }
                    }
                })));

            return createdPlan.Id;
        }

        private async Task<string> CreateSubscriptionAsync(string planId)
        {
            var subscription = new
            {
                plan_id = planId,
                start_time = DateTime.UtcNow.AddMinutes(1).ToString("o"),
                application_context = new
                {
                    brand_name = "ClientTrackPro",
                    locale = "en-US",
                    shipping_preference = "NO_SHIPPING",
                    user_action = "SUBSCRIBE_NOW",
                    return_url = "https://clienttrackpro.com/subscription/success",
                    cancel_url = "https://clienttrackpro.com/subscription/cancel"
                }
            };

            var response = await _httpClient.PostAsync(
                "/v1/billing/subscriptions",
                new StringContent(JsonSerializer.Serialize(subscription)));

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to create subscription");

            var content = await response.Content.ReadAsStringAsync();
            var createdSubscription = JsonSerializer.Deserialize<PayPalSubscription>(content);

            return createdSubscription.Id;
        }
    }

    public class PayPalToken
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }
    }

    public class PayPalPayment
    {
        public string Id { get; set; }
        public string State { get; set; }
        public PayPalPayer Payer { get; set; }
    }

    public class PayPalPayer
    {
        public string Status { get; set; }
    }

    public class PayPalPlan
    {
        public string Id { get; set; }
    }

    public class PayPalSubscription
    {
        public string Id { get; set; }
    }
} 