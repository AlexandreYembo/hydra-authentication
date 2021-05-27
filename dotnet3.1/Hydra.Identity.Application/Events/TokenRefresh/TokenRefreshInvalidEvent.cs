using FluentValidation.Results;
using Hydra.Core.Mediator.Messages;

namespace Hydra.Identity.Application.Events.TokenRefresh {
    public class TokenRefreshInvalidEvent : Event
    {
        public TokenRefreshInvalidEvent () {
            ValidationResult = new ValidationResult();
            ValidationResult.Errors.Add(new ValidationFailure(string.Empty, "Invalid token refresh"));
        }
        public ValidationResult ValidationResult { get; set; }
    }
}