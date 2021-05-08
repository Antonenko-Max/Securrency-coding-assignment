using System.Threading;
using System.Threading.Tasks;
using Securrency.TDS.Web.Controllers;

namespace Securrency.TDS.Web.Services.PaymentService
{
    public interface IPaymentService
    {
        Task UpdatePaymentsAsync(WalletPostModel[] wallets, CancellationToken ct);
    }
}
