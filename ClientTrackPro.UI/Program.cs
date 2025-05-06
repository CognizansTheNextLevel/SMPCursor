using Microsoft.Extensions.DependencyInjection;
using ClientTrackPro.UI.Commands;

namespace ClientTrackPro.UI
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var app = new App();
            await app.InitializeAsync();

            // Create superuser if it doesn't exist
            try
            {
                var command = new CreateSuperUserCommand(App.ServiceProvider);
                await command.ExecuteAsync(
                    email: "shamusno1@gmail.com",
                    username: "shamus",
                    password: "Prosperity2025@#"
                );
                Console.WriteLine("Superuser account created successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating superuser: {ex.Message}");
            }

            app.Run();
        }
    }
} 