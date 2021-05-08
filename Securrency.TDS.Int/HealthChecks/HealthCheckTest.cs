using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Securrency.TDS.WebClient;

namespace Securrency.TDS.Int.HealthChecks
{
    class HealthCheckTest : IntegrationTestBase
    {
        [Test]
        public async Task Test_Health_Endpoint_Works()
        {
            using HttpClient httpClient = Integration.GetTdsHttpClient();
            var client = new TdsClient(httpClient, Integration.CreateLogger<TdsClient>());

            string machineName = await client.GetHealthAsync(CancellationToken.None);
            StringAssert.Contains(Environment.MachineName, machineName);
        }

        [Test]
        public async Task Test_Ready_Endpoint_Works()
        {
            using HttpClient httpClient = Integration.GetTdsHttpClient();
            var client = new TdsClient(httpClient, Integration.CreateLogger<TdsClient>());

            string dbStatus = await client.GetReadyAsync(CancellationToken.None);
            StringAssert.Contains("DB is up to date", dbStatus);
        }

    }
}
