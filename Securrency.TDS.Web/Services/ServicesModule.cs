using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Securrency.TDS.Web.Infrastructure.ClientRetryPolicy;
using Securrency.TDS.Web.Services.PaymentService;
using Securrency.TDS.Web.Services.ReportService;
using Securrency.TDS.Web.Services.StellarService;

namespace Securrency.TDS.Web.Services
{
    public static class ServicesModule
    {
        
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<StellarOptions>().Bind(configuration.GetSection("Stellar")).ValidateDataAnnotations();
            services.AddSingleton<IPaymentService, PaymentServiceImpl>();
            services.AddSingleton<IStellarClient, StellarClient>();
            services.AddSingleton<IReportService, ReportServiceImpl>();

            services.AddHttpClient(StellarClient.CLIENT_NAME).AddDefaultPolicies();
        }
    }
}
