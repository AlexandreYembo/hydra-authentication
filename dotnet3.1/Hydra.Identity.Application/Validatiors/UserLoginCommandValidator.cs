using FluentValidation;
using Hydra.Core.Domain.Validations;
using Hydra.Identity.Application.Commands.UserLogin;

namespace Hydra.Identity.Application.Validatiors
{
    public class UserLoginCommandValidator : AbstractValidator<UserLoginCommand>
    {
        public UserLoginCommandValidator()
        {
             RuleFor(r => r.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .WithErrorCode("-1003")
                .Must(HasValidEmail)
                .WithMessage("Invalid format of the email")
                .WithErrorCode("-1005");
    
            RuleFor(r => r.Password)
                .NotEmpty()
                .WithMessage("Password is required")
                .WithErrorCode("-1004");
        }
        protected static bool HasValidEmail(string email) => Email.IsValid(email);
    }
}