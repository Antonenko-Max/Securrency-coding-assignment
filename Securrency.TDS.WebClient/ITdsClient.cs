using System.Threading;
using System.Threading.Tasks;
using Securrency.TDS.Web.Controllers;

namespace Securrency.TDS.WebClient
{
    public interface ITdsClient
    {
        Task UploadWallets(WalletPostModel[] wallets, CancellationToken ct);
    }
}