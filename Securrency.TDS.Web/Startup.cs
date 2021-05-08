using Securrency.TDS.Web.DataLayer;
using Securrency.TDS.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Securrency.TDS.Web.Services.HealthService;

namespace Securrency.TDS.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDataLayer(Configuration);
            services.AddServices(Configuration);
            services.AddHealthAndReadinessChecks();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseHealthAndReadinessChecks();
            
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}