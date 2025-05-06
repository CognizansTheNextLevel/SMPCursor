using System;
using System.Threading.Tasks;
using ClientTrackPro.Core.Data;
using ClientTrackPro.Core.Models.Account;
using ClientTrackPro.Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClientTrackPro.Core.Scripts
{
    public class CreateSuperUser
    {
        private readonly IServiceProvider _serviceProvider;

        public CreateSuperUser(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task CreateAsync(string email, string username, string password)
        {
            using var scope = _serviceProvider.CreateScope();
            var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();
            var adminService = scope.ServiceProvider.GetRequiredService<IAdminService>();

            // Create user
            var user = await accountService.RegisterAsync(email, username, password);
            if (user == null)
                throw new Exception("Failed to create user");

            // Create admin user
            var adminUser = await adminService.CreateAdminUserAsync(user.Id, "SuperAdmin");
            if (adminUser == null)
                throw new Exception("Failed to create admin user");

            // Update user subscription
            await adminService.UpdateUserSubscriptionAsync(user.Id, "Premium", 36500); // 100 years
        }
    }
} 