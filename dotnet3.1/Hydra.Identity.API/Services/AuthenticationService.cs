using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hydra.Identity.API.Data;
using Hydra.Identity.API.Extensions;
using Hydra.Identity.API.Models;
using Hydra.WebAPI.Core.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.JwtSigningCredentials.Interfaces;

namespace Hydra.Identity.API.Services
{
    public class AuthenticationService
    {
        public readonly SignInManager<IdentityUser> SignInManager;
        public readonly UserManager<IdentityUser> UserManager;
        private readonly ApplicationDbContext _context;
        private readonly IAspNetUser _aspNetUser;
        private readonly IJsonWebKeySetService _jwksService;

        private readonly AppTokenSettings _tokenSettings;

        public AuthenticationService(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context,
            IAspNetUser aspNetUser,
            IJsonWebKeySetService jwksService,
            IOptions<AppTokenSettings> tokenSettings)
        {
            SignInManager = signInManager;
            UserManager = userManager;
            _aspNetUser = aspNetUser;
            _context = context;
            _jwksService = jwksService;
            _tokenSettings = tokenSettings.Value;
        }

        public async Task<UserLoginResponse> TokenGenerator(string email)
        {
            var user = await UserManager.FindByEmailAsync(email);
            var claims = await UserManager.GetClaimsAsync(user);

            var identityClaims = await GetUserClaims(claims, user);
            var encodedToken = EncodeToken(identityClaims);

            var refreshToken = await RefreshToken(email);

            return GetTokenResponse(encodedToken, user, claims, refreshToken);
        }

        private async Task<ClaimsIdentity> GetUserClaims(ICollection<Claim> claims, IdentityUser user)
        {
            var userRoles = await UserManager.GetRolesAsync(user);

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(),
                ClaimValueTypes.Integer64));
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim("role", userRole));
            }

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);

            return identityClaims;
        }

         private string EncodeToken(ClaimsIdentity identityClaims)
        {
            //Endpoint authentication api
            var currentIssuer = _tokenSettings.Issuer //takes from appsetting --> fixed to use the proper issue when deployed using docker.
                                ?? $"{ _aspNetUser.GetHttpContext().Request.Scheme}://{_aspNetUser.GetHttpContext().Request.Host}";

            var tokenHandler = new JwtSecurityTokenHandler();
            var signingCredentials = _jwksService.GetCurrent();
            
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = currentIssuer,
                // Audience = _appSettings.ValidAudience, this is not used because it matters when tokens is valid
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = signingCredentials
            });

            return tokenHandler.WriteToken(token);
        }

       private UserLoginResponse GetTokenResponse(string encodedToken, IdentityUser user, IEnumerable<Claim> claims, RefreshToken refreshToken)
        {
            return new UserLoginResponse
            {
                AccessToken = encodedToken,
                ExpiresIn = TimeSpan.FromHours(1).TotalSeconds,
                RefreshToken = refreshToken.Token,
                UserToken = new UserToken
                {
                    Id = user.Id,
                    Email = user.Email,
                    Claims = claims.Select(c => new UserClaim { Type = c.Type, Value = c.Value })
                }
            };
        }

        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                .TotalSeconds);

        private async Task<RefreshToken> RefreshToken(string email)
        {
            var refreshToken = new RefreshToken
            {
                Username = email,
                ExpirationDate = DateTime.UtcNow.AddHours(_tokenSettings.RefreshTokenExpiration)
            };

            //remote in batch
            _context.RefreshTokens.RemoveRange(_context.RefreshTokens.Where(rt => rt.Username == email));

            await _context.RefreshTokens.AddAsync(refreshToken);

            await _context.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<RefreshToken> GetRefreshToken(Guid refreshToken)
        {
            var token = await _context.RefreshTokens.AsNoTracking()
                                                    .FirstOrDefaultAsync(rt => rt.Token == refreshToken);
            
            return token != null && token.ExpirationDate.ToLocalTime() > DateTime.Now
                ? token
                : null;
        }
    }
}