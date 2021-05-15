using System.Threading;
using System.Threading.Tasks;
using Hydra.Core.Mediator.Abstractions.Mediator;
using Hydra.Identity.Application.Commands.RegisterUser;
using Hydra.Identity.Application.Events.RegisterUser;
using MediatR;

namespace Hydra.Identity.Application.Events
{
    public class RegisterUserEventHandler :
                    INotificationHandler<UserCanceledEvent>,
                    INotificationHandler<CustomerCreatedEvent>
    {
        private readonly IMediatorHandler _mediator;

        public RegisterUserEventHandler(IMediatorHandler mediator)
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