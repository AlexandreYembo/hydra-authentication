using FluentValidation.Results;
using Hydra.Core.Mediator.Messages;

namespace Hydra.Identity.Application.Commands.UserRegister
{
    public class CreateNewUserCommand: Command<ValidationResult>
    {
        public CreateNewUserCommand(string identityNumber, string name, string username, string email, string password)
        {
            IdentityNumber = identityNumber;
            Name = name;
            Username = username;
            Email = email;
            Password = password;
        }

        public string IdentityNumber { get; set; }
        public string Name { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}