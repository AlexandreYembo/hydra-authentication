using System;

namespace Hydra.Identity.Application.Exceptions
{
    public class UserTokenException : Exception
    {
        public UserTokenException(string message) : base(message){ }
    }
}