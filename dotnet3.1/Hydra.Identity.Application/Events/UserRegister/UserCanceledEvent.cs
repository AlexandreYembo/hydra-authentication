using System;
using FluentValidation.Results;
using Hydra.Core.Mediator.Messages;

namespace Hydra.Identity.Application.Events.UserRegister
{

    //it will dispatch a event to the service that will listen the eventsourcing integration
    public class UserCanceledEvent : Event
    {
        public UserCanceledEvent(Guid id, string identityNumber, string name, string email)
        {
            AggregateId = id;
            IdentityNumber = identityNumber;
            Name = name;
            Email = email;
            ValidationResult = new ValidationResult();
        }

        public string IdentityNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public ValidationResult ValidationResult { get; set; }
    }
}