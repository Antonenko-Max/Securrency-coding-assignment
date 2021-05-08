using System.Text.Json;
using NUnit.Framework;

namespace Securrency.TDS.Int
{
    public abstract class IntegrationTestBase
    {
        protected static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        [SetUp]
        public void SetUpIntegration()
        {
            Integration.CaptureTestOutput();
        }

        [TearDown]
        public void TearDownIntegration()
        {
            Integration.ReleaseTestOutput();
            Integration.StellarBackend.Server.Reset();
        }
    }
}
