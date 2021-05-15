using System;
using System.Linq;
using FluentValidation.Results;

namespace Hydra.Identity.Application.Models
{
    public class UserLoginResponse : ValidationResult
    {
        public Guid RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public double ExpiresIn { get; set; }
        public UserToken UserToken { get; set; }

        public bool IsValid => !Errors.Any();
    }
}