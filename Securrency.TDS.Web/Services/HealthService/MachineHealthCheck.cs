using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Securrency.TDS.Web.Services.HealthService
{
    public class MachineHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct)
        {
            return Task.FromResult(HealthCheckResult.Healthy(Environment.MachineName));
        }
    }
}
