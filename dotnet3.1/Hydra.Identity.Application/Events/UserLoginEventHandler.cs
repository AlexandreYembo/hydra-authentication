using System.Threading;
using System.Threading.Tasks;
using Hydra.Identity.Application.Events.UserLogin;
using MediatR;

namespace Hydra.Identity.Application.Events
{
    public class UserLoginEventHandler :
                    INotificationHandler<UserLoginFailedEvent>
    {
        public Task Handle(UserLoginFailedEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}