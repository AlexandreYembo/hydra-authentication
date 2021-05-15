using System;
using System.Threading.Tasks;
using Hydra.Identity.Application.Models;
using Microsoft.AspNetCore.Identity;

namespace Hydra.Identity.Application.Providers
{
    public interface IUserLoginProvider
    {
        Task<SignInResult> UserSignInAsync(string email, string password);
        Task<UserLoginResponse> TokenGenerator(string email, string issuer, double refreshTokenExpiration);
        Task<RefreshToken> GetRefreshToken(Guid refreshToken);
    }
}