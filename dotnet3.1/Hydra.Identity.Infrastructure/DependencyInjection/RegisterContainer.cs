using Hydra.Identity.Application.Providers;
using Hydra.Identity.Infrastructure.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Hydra.Identity.Infrastructure.DependencyInjection
{
    public static class RegisterContainer
    {
        public static void RegisterIdentityInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IUserLoginProvider, UserLoginProvider>();
            services.AddScoped<IUserRegisterProvider, UserRegisterProvider>();
        }
    }
}