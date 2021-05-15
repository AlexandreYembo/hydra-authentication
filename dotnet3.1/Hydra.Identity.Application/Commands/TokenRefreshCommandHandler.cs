using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using Hydra.Core.Mediator.Abstractions.Mediator;
using Hydra.Core.Mediator.Messages;
using Hydra.Identity.Application.Commands.TokenRefresh;
using Hydra.Identity.Application.Events.TokenRefresh;
using Hydra.Identity.Application.Models;
using Hydra.Identity.Application.Providers;
using MediatR;

namespace Hydra.Identity.Application.Commands
{
    public class TokenRefreshCommandHandler : CommandHandler,
                                                IRequestHandler<TokenRefreshCommand, CommandResult<UserLoginResponse>>
    {
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly IMediatorHandler _mediatorHandler;
        private readonly IUserProvider _userProvider;

        public TokenRefreshCommandHandler(IUserLoginProvider userLoginProvider,
                                          IMediatorHandler mediatorHandler,
                                          IUserProvider userProvider)
            {
                _userLoginProvider = userLoginProvider;
                _mediatorHandler = mediatorHandler;
                _userProvider = userProvider;
            }

        public async Task<CommandResult<UserLoginResponse>> Handle(TokenRefreshCommand command, CancellationToken cancellationToken)
        {
            var token = await _userLoginProvider.GetRefreshToken(command.TokenId).ConfigureAwait(false);

            if(token == null)
            {
                var @event = new TokenRefreshInvalidEvent();
                
                await _mediatorHandler.PublishEvent(@event).ConfigureAwait(false);

                return new CommandResult<UserLoginResponse>(@event.ValidationResult);
            }

             var jsonToken = await _userLoginProvider.TokenGenerator(token.Username, _userProvider.Issuer, _userProvider.RefreshTokenExpiration).ConfigureAwait(false);
             return new CommandResult<UserLoginResponse>(jsonToken); 
        }
    }
}