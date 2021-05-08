using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using Securrency.TDS.Test;
using Securrency.TDS.Web;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;
using WireMock.Server;

namespace Securrency.TDS.Int
{
    public static class Integration
    {
        private const int APP_PORT = 50001;
        private const int MOCK_STELLAR_PORT = 50002;

        private static readonly Logger IntegrationLogger = new LoggerConfiguration()
            .MinimumLevel.Warning()
            .MinimumLevel.Override("Securrency.TDS", LogEventLevel.Verbose)
            .Enrich.FromLogContext()
            .WriteTo.Sink<TestSink>()
            .CreateLogger();

        private static readonly ILoggerFactory LoggerFactory = new SerilogLoggerFactory(IntegrationLogger);

        private static IHost _webApplication;

        internal static IntegrationBackend StellarBackend { get; private set; }

        internal static ILogger<T> CreateLogger<T>()
        {
            return new Logger<T>(LoggerFactory);
        }

        public static void StartWebApplication()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development",
                EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("ASPNETCORE_URLS", $"http://::{APP_PORT}",
                EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("Database:ConnectionString", Db.ApplicationConnectionString,
                EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("Stellar:BaseAddress", $"http://localhost:{MOCK_STELLAR_PORT}",
                EnvironmentVariableTarget.Process);

            IHostBuilder builder = Program.GetWebApplicationBuilder(Array.Empty<string>(), IntegrationLogger);
            _webApplication = builder.Build();
            _webApplication.Start();

            StartIntegration();
        }

        public static void StopWebApplication()
        {
            _webApplication.StopAsync(CancellationToken.None).Wait();
            _webApplication.Dispose();
            StopIntegration();
        }

        private static void StartIntegration()
        {
            StellarBackend = new IntegrationBackend();
            StellarBackend.Start(MOCK_STELLAR_PORT);
        }

        private static void StopIntegration()
        {
            StellarBackend.Stop();
        }

        private static TextWriter _testOutput;

        internal static void CaptureTestOutput()
        {
            _testOutput = TestContext.Out;
        }

        internal static void ReleaseTestOutput()
        {
            _testOutput = null;
        }

        private class TestSink : ILogEventSink
        {
            [DebuggerStepThrough]
            public void Emit(LogEvent logEvent)
            {
                TextWriter output = Integration._testOutput;
                if (output == null)
                    return;

                output.Write(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
                output.Write("|");
                output.Write(logEvent.Level);
                output.Write("|");
                logEvent.Properties["SourceContext"].Render(output, "l");
                output.Write("|");
                output.WriteLine(logEvent.RenderMessage());
                if (logEvent.Exception != null)
                    output.WriteLine(logEvent.Exception);
            }
        }
        
        public static HttpClient GetTdsHttpClient()
        {
            return new()
            {
                BaseAddress = new Uri($"http://localhost:{APP_PORT}"),
                Timeout = TimeSpan.FromSeconds(20)
            };
        }
    }

    internal sealed class IntegrationBackend
    {
        public void Start(int port, bool ssl = false)
        {
            Server = WireMockServer.Start(port, ssl);
        }

        public void Stop()
        {
            Server.Stop();
        }

        internal WireMockServer Server { get; set; }
    }

}