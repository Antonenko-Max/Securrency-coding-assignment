using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Securrency.TDS.Web.Controllers;

namespace Securrency.TDS.WebClient
{
    public class TdsClient : ITdsClient
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
            { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public TdsClient(HttpClient httpClient, ILogger<TdsClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task UploadWalletsAsync(WalletPostModel[] wallets, CancellationToken ct)
        {
            _logger.LogDebug("Uploading the list of wallets: {0}", (object)wallets);

            using var content = new StringContent(JsonSerializer.Serialize(wallets, SerializerOptions), Encoding.UTF8,
                "application/json");
            using HttpResponseMessage message = await _httpClient.PostAsync($"/wallets", content, ct);
            await ReadResponseAsync(message);
            _logger.LogDebug("The list of wallets uploaded");
        }

        public async Task<string> DownloadReportAsync(string accountId, CancellationToken ct)
        {
            _logger.LogDebug("Downloading a report for the wallet: {0}", accountId);

            using HttpResponseMessage message = await _httpClient.GetAsync($"/wallets/report/{accountId}", ct);
            string result = await ReadResponseAsync(message);
            _logger.LogDebug("The report downloaded");
            return result;
        }

        public async Task<string> GetHealthAsync(CancellationToken ct)
        {
            _logger.LogDebug("Check the service health");
            using HttpResponseMessage message = await _httpClient.GetAsync("/health", ct);
            string response = await ReadResponseAsync(message);
            return response;
        }

        public async Task<string> GetReadyAsync(CancellationToken ct)
        {
            _logger.LogDebug("Check the service health");
            using HttpResponseMessage message = await _httpClient.GetAsync("/ready", ct);
            string response = await ReadResponseAsync(message);
            return response;
        }

        private async Task<string> ReadResponseAsync(HttpResponseMessage message)
        {
            if (message.IsSuccessStatusCode)
            {
                _logger.LogDebug("Successful response");
                using Stream stream = await message.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(stream);
                string result = reader.ReadToEnd();
                return result;
            }

            _logger.LogDebug("Returned the status \"{0} - {1}\" - throwing", message.StatusCode,
                message.ReasonPhrase);

            throw new TdsClientException($"The service returned an error, see the details", message);
        }
    }
}