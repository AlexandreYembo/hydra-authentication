using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hydra.Identity.Application.Exceptions;
using Hydra.Identity.Application.Models;
using Hydra.Identity.Application.Providers;
using Hydra.Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.JwtSigningCredentials.Interfaces;

namespace Hydra.Identity.Infrastructure.Providers
{
    public class UserLoginProvider : IUserLoginProvider
    {
        private readonly UserManager<IdentityUser> _userManager;
         public readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IJsonWebKeySetService _jwksService;

        public UserLoginProvider(UserManager<IdentityUser> userManager,
                                SignInManager<IdentityUser> signInManager,
                                ApplicationDbContext context,
                                IJsonWebKeySetService jwksService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _jwksService = jwksService;
        }

        public async Task<SignInResult> UserSignInAsync(string email, string password)
        {
            var userLogged = await _signInManager.PasswordSignInAsync(email, password, false, true).ConfigureAwait(false);
            return userLogged;
        }
        
        public async Task<UserLoginResponse> TokenGenerator(string email, string issuer, double refreshTokenExpiration)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                var claims = await _userManager.GetClaimsAsync(user);

                var identityClaims = await GetUserClaims(claims, user);
                var encodedToken = EncodeToken(identityClaims, issuer);
                
                var refreshToken = await RefreshToken(email, refreshTokenExpiration);

                return GetTokenResponse(encodedToken, user, claims, refreshToken);
            }
            catch (Exception ex)
            {
                throw new UserTokenException(ex.Message);
            }
          
        }

        public async Task<RefreshToken> GetRefreshToken(Guid refreshToken)
        {
            var token = await _context.RefreshTokens.AsNoTracking()
                                                    .FirstOrDefaultAsync(rt => rt.Token == refreshToken)
                                                    .ConfigureAwait(false);
            
            return token != null && token.ExpirationDate.ToLocalTime() > DateTime.Now
                ? token
                : null;
        }
        

        private async Task<ClaimsIdentity> GetUserClaims(ICollection<Claim> claims, IdentityUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

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

        private async Task<RefreshToken> RefreshToken(string email, double refreshTokenExpiration)
        {
            var refreshToken = new RefreshToken
            {
                Username = email,
                ExpirationDate = DateTime.UtcNow.AddHours(refreshTokenExpiration)
            };

            //remote in batch
            _context.RefreshTokens.RemoveRange(_context.RefreshTokens.Where(rt => rt.Username == email));

            await _context.RefreshTokens.AddAsync(refreshToken).ConfigureAwait(false);

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return refreshToken;
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
        private string EncodeToken(ClaimsIdentity identityClaims, string issuer)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var signingCredentials = _jwksService.GetCurrent();
            
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = issuer,
                // Audience = _appSettings.ValidAudience, this is not used because it matters when tokens is valid
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = signingCredentials
            });

            return tokenHandler.WriteToken(token);
        }

        private static long ToUnixEpochDate(DateTime date)
           => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
               .TotalSeconds);
    }
}