using System.Threading;
using System.Threading.Tasks;
using Securrency.TDS.Web.Controllers;

namespace Securrency.TDS.WebClient
{
    public interface ITdsClient
    {
        Task UploadWalletsAsync(WalletPostModel[] wallets, CancellationToken ct);

        Task<string> DownloadReportAsync(string accountId, CancellationToken ct);
    }
}