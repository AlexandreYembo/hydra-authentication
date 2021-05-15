using FluentValidation.Results;
using Hydra.Core.Mediator.Abstractions.Mediator;
using Hydra.Core.Mediator.Communication;
using Hydra.Core.Mediator.Configuration;
using Hydra.Core.Mediator.Messages;
using Hydra.Identity.Application.Commands;
using Hydra.Identity.Application.Commands.TokenRefresh;
using Hydra.Identity.Application.Commands.UserLogin;
using Hydra.Identity.Application.Commands.UserRegister;
using Hydra.Identity.Application.Events;
using Hydra.Identity.Application.Events.TokenRefresh;
using Hydra.Identity.Application.Events.UserLogin;
using Hydra.Identity.Application.Events.UserRegister;
using Hydra.Identity.Application.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace Hydra.Identity.Application.DependencyInjections
{
    public static class RegisterContainers
    {
        public static void RegisterDomain<TStartup>(this IServiceCollection services)
        {
            services.AddScoped<IMediatorHandler, MediatorHandler>()
                    .RegisterContainerUserRegister()
                    .RegisterContainerUserLogin()
                    .RegisterContainerTokenRefresh()
                    .AddMediator<TStartup>();
        }

        private static IServiceCollection RegisterContainerUserRegister(this IServiceCollection services)
        {
            services.AddScoped<IRequestHandler<CreateNewUserCommand, CommandResult<ValidationResult>>, UserRegisterCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteUserCommand, bool>, UserRegisterCommandHandler>();
            services.AddScoped<IRequestHandler<NotifyNewUserCommand, bool>, UserRegisterCommandHandler>();

            services.AddScoped<INotificationHandler<UserCanceledEvent>, UserRegisterEventHandler>();
            services.AddScoped<INotificationHandler<CustomerCreatedEvent>, UserRegisterEventHandler>();
            return services;
        }

         private static IServiceCollection RegisterContainerUserLogin(this IServiceCollection services)
         {
            services.AddScoped<IRequestHandler<UserLoginCommand, CommandResult<UserLoginResponse>>, UserLoginCommandHandler>();

            services.AddScoped<INotificationHandler<UserLoginFailedEvent>, UserLoginEventHandler>();
            return services;
         }

          private static IServiceCollection RegisterContainerTokenRefresh(this IServiceCollection services)
         {
            services.AddScoped<IRequestHandler<TokenRefreshCommand, CommandResult<UserLoginResponse>>, TokenRefreshCommandHandler>();

            services.AddScoped<INotificationHandler<TokenRefreshInvalidEvent>, TokenRefreshInvalidEventHandler>();
            return services;
         }
    }
}