using System;
using System.Threading.Tasks;
using Hydra.Identity.Application.Models;

namespace Hydra.Identity.Application.Providers
{
    public interface IUserLoginProvider
    {
         Task<UserLoginResponse> TokenGenerator(string email, string issuer, double refreshTokenExpiration);
         Task<RefreshToken> GetRefreshToken(Guid refreshToken);
         
    }
}