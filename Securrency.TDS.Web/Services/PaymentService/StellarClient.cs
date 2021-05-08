using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Securrency.TDS.Web.Controllers;
using Securrency.TDS.Web.Services.StellarService;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.requests;
using stellar_dotnet_sdk.responses.operations;
using stellar_dotnet_sdk.responses.page;

namespace Securrency.TDS.Web.Services.PaymentService
{
    public class StellarClient : IStellarClient
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger _logger;
        private readonly StellarOptions _options;

        internal const string CLIENT_NAME = "Stellar";

        public StellarClient(
            IHttpClientFactory clientFactory, 
            ILogger<StellarClient> logger, 
            IOptions<StellarOptions> options)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _options = options.Value;
        }

        public async Task<List<OperationResponse>[]> GetPaymentsInNativeAssetAsync(WalletPostModel[] wallets, CancellationToken ct)
        {
            _logger.LogDebug("Downloading operations for wallets: {0}", (object) wallets);
            var server = new Server(_options.BaseAddress, _clientFactory.CreateClient(CLIENT_NAME));
            
            Task<Page<OperationResponse>>[] detailTasks = wallets.Select(w => server.Payments.ForAccount(w.AccountId).Execute()).ToArray();

            try
            {
                await Task.WhenAll(detailTasks);
            }
            catch (HttpResponseException e)
            {
                _logger.LogDebug("Abnormal response: {0}", e.Message);
                if (e.StatusCode == 400) throw new ApplicationException("Invalid Account Id");
                throw;
            }

            List<OperationResponse>[] result = detailTasks.Select((t => t.Result.Records)).ToArray();
            
            _logger.LogDebug("{0} lists of operations found", result.Length);

            return result;
        }
    }
}
