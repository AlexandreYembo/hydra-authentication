using Hydra.Identity.API.Services;
using Hydra.WebAPI.Core.User;
using Microsoft.Extensions.DependencyInjection;

namespace Hydra.Identity.API.Configuration
{
    public static class DependencyInjection
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<AuthenticationService>();
            services.AddScoped<IAspNetUser, AspNetUser>();

        }
    }
}