using FluentValidation.Results;
using Hydra.Core.Mediator.Messages;
using Hydra.Identity.Application.Models;
using Hydra.Identity.Application.Validatiors;

namespace Hydra.Identity.Application.Commands.UserLogin
{
    public class UserLoginCommand : Command<UserLoginResponse>
    {
        public UserLoginCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; set; }
        public string Password { get; set; }

         public override bool IsValid()
        {
            ValidationResult = new UserLoginCommandValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}