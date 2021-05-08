using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Securrency.TDS.Web.DataLayer
{
    public class DesignTimeContextFactory: IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            IServiceCollection services = PrepareDesignTimeServices(args);
            ServiceProvider provider = services.BuildServiceProvider();
            var context = provider.GetRequiredService<AppDbContext>();
            return context;
        }

        internal static IServiceCollection PrepareDesignTimeServices(string[] args)
        {
            IConfiguration hostConfiguration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            string environment = hostConfiguration[HostDefaults.EnvironmentKey] ?? Environments.Production;
            IConfiguration appConfiguration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environment}.json", true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var services = new ServiceCollection();
            services.AddLogging(logging =>
            {
                logging.AddConfiguration(appConfiguration.GetSection("Logging"));
                logging.AddConsole();
            });
            services.AddDataLayer(appConfiguration);

            return services;
        }
    }
}
