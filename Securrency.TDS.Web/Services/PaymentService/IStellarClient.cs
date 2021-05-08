using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Securrency.TDS.Web.Controllers;
using stellar_dotnet_sdk.responses.operations;

namespace Securrency.TDS.Web.Services.PaymentService
{
    public interface IStellarClient
    {
        Task<List<OperationResponse>[]> GetPaymentsInNativeAssetAsync(WalletPostModel[] wallets, CancellationToken ct);
    }
}
