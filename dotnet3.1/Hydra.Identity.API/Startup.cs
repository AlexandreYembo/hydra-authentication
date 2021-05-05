using Hydra.Core.API.Setups;
using Hydra.Identity.API.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hydra.Identity.API
{
    public class Startup
    {
        public Startup(IHostEnvironment hostEnvironment)
        {
           Configuration = HostEnvironmentConfiguration.AddHostEnvironment(hostEnvironment);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApiConfiguration(Configuration);
            services.AddSwagger();
            services.RegisterServices();
            services.AddMessageBusConfiguration(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseApiConfiguration(env);
        }
    }
}
