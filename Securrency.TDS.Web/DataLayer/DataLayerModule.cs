using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Securrency.TDS.Web.DataLayer
{
    internal static class DataLayerModule
    {
        public static void AddDataLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DataLayerOptions>(DataLayerOptions.From(configuration));
            services.AddDbContext<AppDbContext>(o => 
                o.AddInterceptors(new QueryCommandInterceptor()), ServiceLifetime.Transient);
            services.AddSingleton<IDbContextFactory, DbContextFactory>();
        }
    }
}