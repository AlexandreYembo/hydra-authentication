using System;
using System.Linq;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace Hydra.Identity.Application.Models
{
    public class UserLoginResponse
    {
        public Guid RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public double ExpiresIn { get; set; }
        public UserToken UserToken { get; set; }

        private ValidationResult ValidationResult {get; set;} = new ValidationResult();

        public UserLoginResponse() { }
        public UserLoginResponse(string message)
        {
            ValidationResult.Errors.Add(new ValidationFailure(string.Empty, message));
        }

        public UserLoginResponse(SignInResult result)
        {
            var error = "User or password invalid!";

            if(result.IsLockedOut)
                error = "User temporary locked for many tries!";

            if(result.IsNotAllowed)
                error = "Is Not Allowed!";

            if(result.RequiresTwoFactor)
                error = "Requires Two Factor Authenticator!";

            ValidationResult = new ValidationResult();
            ValidationResult.Errors.Add(new ValidationFailure(string.Empty, error));
        }
        public bool IsValid => !ValidationResult.Errors.Any();
        public ValidationResult GetValidationResult() => ValidationResult;
    }
}