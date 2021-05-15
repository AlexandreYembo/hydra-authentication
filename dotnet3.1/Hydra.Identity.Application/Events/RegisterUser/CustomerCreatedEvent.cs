using System;
using Hydra.Core.Mediator.Messages;

namespace Hydra.Identity.Application.Events.RegisterUser
{
    public class CustomerCreatedEvent : Event
    {
        public CustomerCreatedEvent(Guid id, string name, string email)
        {
            AggregateId = id;
            Name = name;
            Email = email;
        }

        public string Name { get; set; }
        public string Email { get; set; }
    }
}