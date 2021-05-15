using FluentValidation.Results;
using Hydra.Core.Mediator.Abstractions.Mediator;
using Hydra.Core.Mediator.Communication;
using Hydra.Core.Mediator.Configuration;
using Hydra.Core.Mediator.Messages;
using Hydra.Identity.Application.Commands;
using Hydra.Identity.Application.Commands.RegisterUser;
using Hydra.Identity.Application.Events;
using Hydra.Identity.Application.Events.RegisterUser;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace Hydra.Identity.Application.DependencyInjections
{
    public static class RegisterContainers
    {
         public static void RegisterDomain<TStartup>(this IServiceCollection services)
        {
            services.AddScoped<IMediatorHandler, MediatorHandler>();
            
            services.AddScoped<IRequestHandler<CreateNewUserCommand, CommandResult<ValidationResult>>, RegisterUserCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteUserCommand, bool>, RegisterUserCommandHandler>();
            services.AddScoped<IRequestHandler<NotifyNewUserCommand, bool>, RegisterUserCommandHandler>();

            services.AddScoped<INotificationHandler<UserCanceledEvent>, RegisterUserEventHandler>();
            services.AddScoped<INotificationHandler<CustomerCreatedEvent>, RegisterUserEventHandler>();

           services.AddMediator<TStartup>();
        }
    }
}