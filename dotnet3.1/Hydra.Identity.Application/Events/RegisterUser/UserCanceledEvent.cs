using System;
using FluentValidation.Results;
using Hydra.Core.Mediator.Messages;

namespace Hydra.Identity.Application.Events.RegisterUser
{
    public class UserCanceledEvent : Event
    {
        public UserCanceledEvent(string identityNumber, string name, string email)
        {
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