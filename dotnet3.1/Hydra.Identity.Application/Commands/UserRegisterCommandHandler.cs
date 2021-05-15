using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using Hydra.Core.Mediator.Abstractions.Mediator;
using Hydra.Core.Mediator.Integration;
using Hydra.Core.Mediator.Messages;
using Hydra.Core.MessageBus;
using Hydra.Identity.Application.Commands.UserRegister;
using Hydra.Identity.Application.Events.UserRegister;
using Hydra.Identity.Application.Providers;
using Hydra.User.Integration.Messages;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Hydra.Identity.Application.Commands
{
    public class UserRegisterCommandHandler : CommandHandler,
                                       IRequestHandler<CreateNewUserCommand, CommandResult<ValidationResult>>,
                                       IRequestHandler<NotifyNewUserCommand, bool>,
                                       IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly IMessageBus _messageBus;
        private readonly IMediatorHandler _mediator;
        private readonly IUserRegisterProvider _userRegisterProvider;

        public UserRegisterCommandHandler(
                              IUserRegisterProvider userRegisterProvider,
                              IMediatorHandler mediator,
                              IMessageBus messageBus)
        {
          _userRegisterProvider = userRegisterProvider;
          _mediator = mediator;
          _messageBus = messageBus;
        }

        public async Task<CommandResult<ValidationResult>> Handle(CreateNewUserCommand message, CancellationToken cancellationToken)
        {
          var user = new IdentityUser
          {
            UserName = message.Email,
            Email = message.Email,
            EmailConfirmed = false // Will send email to confirm the account
          };

          var userCreated = await _userRegisterProvider.CreateUserAsync(user, message.Password).ConfigureAwait(false);
           
          if(userCreated.Succeeded)
          {
            var result = await CreateCustomer(message);

            return new CommandResult<ValidationResult>(result.ValidResult);
          }

          return new CommandResult<ValidationResult>(new ValidationResult(userCreated.Errors.Select(e => new ValidationFailure(e.Code, e.Description))));
        }

        public async Task<bool> Handle(DeleteUserCommand message, CancellationToken cancellationToken)
        {
            var user = await _userRegisterProvider.FindByEmailAsync(message.Email).ConfigureAwait(false);
            var result = await _userRegisterProvider.DeleteUserAsync(user).ConfigureAwait(false);
            return result.Succeeded;
        }


        /// <summary>
        /// Will dispatch a command to a service that will notify that the new user was created
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(NotifyNewUserCommand message, CancellationToken cancellationToken)
        {
            var notificationResult = new UserNotificationIntegrationEvent(message.AggregateId, message.Email, message.Name);
            await _messageBus.PublishAsync<UserNotificationIntegrationEvent>(notificationResult).ConfigureAwait(false);
            return true;
        }

        private async Task<ResponseMessage> CreateCustomer(CreateNewUserCommand message)
        {
            var user = await _userRegisterProvider.FindByEmailAsync(message.Email).ConfigureAwait(false);
            var userSaved =  new UserSaveIntegrationEvent(Guid.Parse(user.Id), message.Name, user.Email, message.IdentityNumber);
            try
            {
                var result = await _messageBus.RequestAsync<UserSaveIntegrationEvent, ResponseMessage>(userSaved).ConfigureAwait(false);

                if(!result.IsValid())
                {
                    var @event = new UserCanceledEvent(message.IdentityNumber, message.Name, message.Email);
                    @event.ValidationResult = result.ValidResult;
                    await _mediator.PublishEvent(@event);
                    return result;
                }
                await _mediator.PublishEvent(new CustomerCreatedEvent(result.AggregateId, message.Name, message.Email)).ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                var @event = new UserCanceledEvent(message.IdentityNumber, message.Name, message.Email);
                @event.ValidationResult.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, ex.Message));
                await _mediator.PublishEvent(@event).ConfigureAwait(false);

                return new ResponseMessage(@event.ValidationResult);
            }
        }
    }
}