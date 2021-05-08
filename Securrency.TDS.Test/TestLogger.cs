using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace Securrency.TDS.Test
{
    internal static class TestLogger
    {
        internal static readonly ILoggerFactory Factory = new SerilogLoggerFactory(new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console(
                outputTemplate: "{Timestamp:HH:mm:ss}|{Level:u3}|{SourceContext}|{Message:lj}{NewLine}{Exception}")
            .CreateLogger(), true);
    }
}
