using System;
using Hydra.Core.Mediator.Messages;

namespace Hydra.Identity.Application.Commands.UserRegister
{
    public class NotifyNewUserCommand : Command
    {
     public NotifyNewUserCommand(Guid id, string name, string email)
        {
            AggregateId = id;
            Name = name;
            Email = email;
        }

        public string Name { get; set; }
        public string Email { get; set; }
    }
}