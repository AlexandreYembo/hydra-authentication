using System.Threading.Tasks;
using Hydra.Identity.Application.Providers;
using Microsoft.AspNetCore.Identity;

namespace Hydra.Identity.Infrastructure.Providers
{
    public class UserRegisterProvider : IUserRegisterProvider
    {
        private readonly UserManager<IdentityUser> _userManager;


        public UserRegisterProvider(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> CreateUserAsync(IdentityUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password).ConfigureAwait(false);
            return result;
        }

        public async Task<IdentityUser> FindByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
            return user;
        }

        public async Task<IdentityResult> DeleteUserAsync(IdentityUser user){
            var result = await _userManager.DeleteAsync(user).ConfigureAwait(false);
            return result;
        }
    }
}