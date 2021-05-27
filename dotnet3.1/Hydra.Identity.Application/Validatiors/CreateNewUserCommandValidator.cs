using FluentValidation;
using Hydra.Core.Domain.Validations;
using Hydra.Identity.Application.Commands.UserRegister;

namespace Hydra.Identity.Application.Validators
{
    public class CreateNewUserCommandValidator: AbstractValidator<CreateNewUserCommand>
    {
        public CreateNewUserCommandValidator()
        {
            RuleFor(r => r.IdentityNumber)
                .NotEmpty()
                .WithMessage("Identity Number is required")
                .WithErrorCode("-1000");

            RuleFor(r => r.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .WithErrorCode("-1001");

            RuleFor(r => r.Username)
                .NotEmpty()
                .WithMessage("Username is required")
                .WithErrorCode("-1002")
                .Must(HasValidEmail)
                .WithMessage("Invalid format of the email")
                .WithErrorCode("-1005");

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