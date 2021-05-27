using Hydra.Core.API.User;
using Hydra.Identity.API.Extensions;
using Hydra.Identity.Application.Providers;
using Microsoft.Extensions.Options;

namespace Hydra.Identity.API.Providers
{
    public class UserProvider : IUserProvider
    {
        private readonly IAspNetUser _aspNetUser;
        private readonly AppTokenSettings _tokenSettings;

        public UserProvider(IAspNetUser aspNetUser,
                            IOptions<AppTokenSettings> tokenSettings)
        {
            _aspNetUser = aspNetUser;
            _tokenSettings = tokenSettings.Value;
        }
          
        public string Issuer => _tokenSettings.Issuer //takes from appsetting --> fixed to use the proper issue when deployed using docker.
                                ?? $"{ _aspNetUser.GetHttpContext().Request.Scheme}://{_aspNetUser.GetHttpContext().Request.Host}";
        public double RefreshTokenExpiration => _tokenSettings.RefreshTokenExpiration;
    }
}