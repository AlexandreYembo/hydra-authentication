using System.Threading;
using System.Threading.Tasks;
using Hydra.Identity.Application.Events.TokenRefresh;
using MediatR;

namespace Hydra.Identity.Application.Events
{
    public class TokenRefreshInvalidEventHandler :
                        INotificationHandler<TokenRefreshInvalidEvent>
    {
        public Task Handle(TokenRefreshInvalidEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}