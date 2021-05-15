using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using Hydra.Core.Mediator.Abstractions.Mediator;
using Hydra.Core.Mediator.Messages;
using Hydra.Identity.Application.Commands.UserLogin;
using Hydra.Identity.Application.Events.UserLogin;
using Hydra.Identity.Application.Exceptions;
using Hydra.Identity.Application.Models;
using Hydra.Identity.Application.Providers;
using MediatR;

namespace Hydra.Identity.Application.Commands
{
    public class UserLoginCommandHandler : CommandHandler,
                                        IRequestHandler<UserLoginCommand, CommandResult<UserLoginResponse>>
    {
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly IMediatorHandler _mediatorHandler;

        private readonly IUserProvider _userProvider;

        public UserLoginCommandHandler(IUserLoginProvider userLoginProvider,
                                       IMediatorHandler mediatorHandler,
                                       IUserProvider userProvider)
        {
            _userLoginProvider = userLoginProvider;
            _mediatorHandler = mediatorHandler;
            _userProvider = userProvider;
        }


        public async Task<CommandResult<UserLoginResponse>> Handle(UserLoginCommand command, CancellationToken cancellationToken)
        {
            var userLogged = await _userLoginProvider.UserSignInAsync(command.Email, command.Password);

            if(!userLogged.Succeeded)
            {
                var @event = new UserLoginFailedEvent(command.Email, userLogged);
                await _mediatorHandler.PublishEvent(@event).ConfigureAwait(false);
                return new CommandResult<UserLoginResponse>(@event.ValidationResult);
            }

            try
            {
                var jsonToken = await _userLoginProvider.TokenGenerator(command.Email, _userProvider.Issuer, _userProvider.RefreshTokenExpiration).ConfigureAwait(false);
                return new CommandResult<UserLoginResponse>(jsonToken);
            }
            catch (UserTokenException ute)
            {
                var @event = new UserLoginFailedEvent(command.Email, ute.Message);
                await _mediatorHandler.PublishEvent(@event).ConfigureAwait(false);
                return new CommandResult<UserLoginResponse>(@event.ValidationResult);
            }
        }
    }
}