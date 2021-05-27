using Hydra.Core.API.User;
using Hydra.Identity.API.Providers;
using Hydra.Identity.Application.DependencyInjections;
using Hydra.Identity.Application.Providers;
using Hydra.Identity.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Hydra.Identity.API.Configuration
{
    public static class DependencyInjection
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.RegisterIdentityInfrastructure();
            services.AddScoped<IAspNetUser, AspNetUser>();
            services.AddScoped<IUserProvider, UserProvider>();
            services.RegisterDomain<Startup>();
        }
    }
}