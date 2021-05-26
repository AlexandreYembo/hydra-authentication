using System;
using FluentValidation.Results;
using Hydra.Core.Mediator.Messages;
using Microsoft.AspNetCore.Identity;

namespace Hydra.Identity.Application.Events.UserLogin
{
    public class UserLoginFailedEvent : Event
    {
        public string UserName { get; set; }
        public ValidationResult ValidationResult { get; set; }

        public UserLoginFailedEvent(string userName, string message)
        {
            AggregateId = Guid.NewGuid();
            UserName = userName;
            ValidationResult = new ValidationResult();
            ValidationResult.Errors.Add(new ValidationFailure(string.Empty, message));
        }

        public UserLoginFailedEvent(string userName, SignInResult result)
        {
            AggregateId = Guid.NewGuid();
            UserName = userName;
            var error = "User or password invalid!";

            if(result.IsLockedOut)
                error = "User temporary locked for many tries!";

            if(result.IsNotAllowed)
                error = "Is Not Allowed!";

            if(result.RequiresTwoFactor)
                error = "Requires Two Factor Authenticator!";

            ValidationResult = new ValidationResult();
            ValidationResult.Errors.Add(new ValidationFailure(string.Empty, error));
        }
    }
}