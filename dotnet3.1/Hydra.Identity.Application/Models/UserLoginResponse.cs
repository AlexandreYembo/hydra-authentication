using System;

namespace Hydra.Identity.Application.Models
{
    public class UserLoginResponse
    {
        public Guid RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public double ExpiresIn { get; set; }
        public UserToken UserToken { get; set; }
    }
}