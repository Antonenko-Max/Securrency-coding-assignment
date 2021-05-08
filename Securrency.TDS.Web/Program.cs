using System;
using System.Linq;
using Securrency.TDS.Web.DataLayer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using ILogger = Serilog.ILogger;

namespace Securrency.TDS.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            static bool Migrate(string p)
            {
                return string.Equals(p, "--migrate", StringComparison.InvariantCultureIgnoreCase);
            }

            if (args.FirstOrDefault(Migrate) == null)
                GetWebApplicationBuilder(args).Build().Run();
            else
                RunMigrations(args);
        }

        public static IHostBuilder GetWebApplicationBuilder(string[] args, ILogger logger = null)
        {
            ILogger serilog = logger ?? CreateLogger();
            IHostBuilder host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .UseSerilog(serilog);
            return host;
        }

        private static void RunMigrations(string[] args)
        {
            IServiceCollection services = DesignTimeContextFactory.PrepareDesignTimeServices(args);
            services.AddSingleton<Migrations>();

            IServiceProvider provider = services.BuildServiceProvider();
            provider.GetRequiredService<Migrations>().Run();
        }

        private static Logger CreateLogger()
        {
            return new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate:
                    "{Timestamp:yyyy-MM-ddTHH:mm:ss}|{Level:u3}|{SourceContext}|{Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }
    }
}