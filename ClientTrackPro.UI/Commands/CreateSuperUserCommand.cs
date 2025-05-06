using System;
using System.Threading.Tasks;
using ClientTrackPro.Core.Scripts;
using Microsoft.Extensions.DependencyInjection;

namespace ClientTrackPro.UI.Commands
{
    public class CreateSuperUserCommand
    {
        private readonly IServiceProvider _serviceProvider;

        public CreateSuperUserCommand(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ExecuteAsync(string email, string username, string password)
        {
            using var scope = _serviceProvider.CreateScope();
            var script = new CreateSuperUser(scope.ServiceProvider);
            await script.CreateAsync(email, username, password);
        }
    }
} 