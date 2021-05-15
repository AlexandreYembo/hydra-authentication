using Hydra.Core.Mediator.Messages;

namespace Hydra.Identity.Application.Commands.UserRegister
{
    public class DeleteUserCommand : Command
    {
        public DeleteUserCommand(string email)
        {
            Email = email;
        }
        public string Email { get; set; }
    }
}