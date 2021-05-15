using System;
using System.Threading.Tasks;
using FluentValidation.Results;
using Hydra.Core.Mediator.Abstractions.Mediator;
using Hydra.Identity.Application.Commands.RegisterUser;
using Microsoft.Extensions.DependencyInjection;

namespace Hydra.Identity.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = RegisterStartup();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var mediator = serviceProvider.GetService<IMediatorHandler>();

            var command = new CreateNewUserCommand("123", "trterewr", "fds@sd.com", "12312312312@2qdsada.com", "31312bdfdf32r231");
            var result = await mediator.SendCommand<CreateNewUserCommand, ValidationResult>(command).ConfigureAwait(false);

        }

        private static ServiceCollection RegisterStartup()
        {
            var serviceCollection = new ServiceCollection();
            Startup.Main(serviceCollection);

            return serviceCollection;
        }
    }
}
