using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Hydra.Identity.Application.Providers
{
    public interface IUserRegisterProvider
    {
        Task<IdentityResult> CreateUserAsync(IdentityUser user, string password);
        Task<IdentityUser> FindByEmailAsync(string email);
        Task<IdentityResult> DeleteUserAsync(IdentityUser user);
    }
}