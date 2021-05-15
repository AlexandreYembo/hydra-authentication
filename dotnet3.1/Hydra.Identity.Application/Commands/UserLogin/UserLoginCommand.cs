using FluentValidation.Results;
using Hydra.Core.Mediator.Messages;
using Hydra.Identity.Application.Models;

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
    }
}