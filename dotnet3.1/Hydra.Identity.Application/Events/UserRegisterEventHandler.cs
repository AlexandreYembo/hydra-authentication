using System.Threading;
using System.Threading.Tasks;
using Hydra.Core.Mediator.Abstractions.Mediator;
using Hydra.Identity.Application.Commands.UserRegister;
using Hydra.Identity.Application.Events.UserRegister;
using MediatR;

namespace Hydra.Identity.Application.Events
{
    public class UserRegisterEventHandler :
                    INotificationHandler<UserCanceledEvent>,
                    INotificationHandler<CustomerCreatedEvent>
    {
        private readonly IMediatorHandler _mediator;

        public UserRegisterEventHandler(IMediatorHandler mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(UserCanceledEvent notification, CancellationToken cancellationToken)
        {
          await _mediator.SendCommand(new DeleteUserCommand(notification.Email));
        }

        public async Task Handle(CustomerCreatedEvent notification, CancellationToken cancellationToken)
        {
            await _mediator.SendCommand(new NotifyNewUserCommand(notification.AggregateId, notification.Name, notification.Email)).ConfigureAwait(false);
        }
    }
}