using Hydra.Core.API.User;
using Hydra.Identity.Application.DependencyInjections;
using Hydra.Identity.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hydra.Identity.Console
{
    public class Startup
    {
        public static IConfigurationRoot configuration;

        public static void Main(IServiceCollection services)
        {
           services.RegisterIdentityInfrastructure();
            services.AddScoped<IAspNetUser, AspNetUser>();
            services.RegisterDomain<Startup>();
        }
    }
}