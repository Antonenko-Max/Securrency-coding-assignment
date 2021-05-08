using System.Threading;
using System.Threading.Tasks;

namespace Securrency.TDS.Web.Services.ReportService
{
    public interface IReportService
    {
        Task<WalletReportLine[]> GenerateReportAsync(string accountId, CancellationToken ct);
    }
}
