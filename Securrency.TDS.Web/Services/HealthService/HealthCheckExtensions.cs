using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Securrency.TDS.Web.Services.HealthService
{
    public static class HealthCheckExtensions
    {
        public static void AddHealthAndReadinessChecks(this IServiceCollection services)
        {
            services.AddHealthChecks().AddCheck<MachineHealthCheck>("health", tags: new[] { "health" });
            services.AddHealthChecks().AddCheck<DbHealthCheckService>("db", tags: new[] { "db" });
        }

        public static void UseHealthAndReadinessChecks(this IApplicationBuilder app)
        {
            app.UseHealthChecks((PathString)"/health", new HealthCheckOptions()
            {
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = 200,
                    [HealthStatus.Degraded] = 200,
                    [HealthStatus.Unhealthy] = 503
                },
                ResponseWriter = ResponseWriteHelper.WriteResponse,
                Predicate = (check) => check.Tags.Contains("health")
            });
            app.UseHealthChecks((PathString)"/ready", new HealthCheckOptions()
            {
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = 200,
                    [HealthStatus.Degraded] = 200,
                    [HealthStatus.Unhealthy] = 503
                },
                ResponseWriter = ResponseWriteHelper.WriteResponse,
                Predicate = (check) => check.Tags.Contains("db")
            });

        }


    }
}
