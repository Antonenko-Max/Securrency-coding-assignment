using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Securrency.TDS.Web.DataLayer;
using Securrency.TDS.Web.DataLayer.Entities;

namespace Securrency.TDS.Web.Services.ReportService
{
    public class ReportServiceImpl : IReportService
    {
        private readonly ILogger _logger;
        private readonly IDbContextFactory _dbContextFactory;

        public ReportServiceImpl(
            ILogger<ReportServiceImpl> logger,
            IDbContextFactory dbContextFactory)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
        }

        public async Task<WalletReportLine[]> GenerateReportAsync(string accountId, CancellationToken ct)
        {
            using TransactionScope tran = Tran.BeginScope(IsolationLevel.ReadCommitted);
            using AppDbContext ctx = _dbContextFactory.CreateContext();

            _logger.LogDebug("Trying calculate balance");

            WalletReportLine[] income = await ctx.Set<PaymentEntity>()
                .Where(p => p.To == accountId && p.TransactionSuccessful)
                .GroupBy(g => g.From)
                .Select(s => new WalletReportLine() { AccountId = s.Key, Amount = s.Sum(r => r.Amount) })
                .ToArrayAsync(ct);

            WalletReportLine[] outcome = await ctx.Set<PaymentEntity>()
                .Where(p => p.From == accountId && p.TransactionSuccessful)
                .GroupBy(g => g.To)
                .Select(s => new WalletReportLine() { AccountId = s.Key, Amount = -s.Sum(r => r.Amount) })
                .ToArrayAsync(ct);

            WalletReportLine[] report =
                income.Union(outcome)
                    .GroupBy(g => g.AccountId)
                    .Select(s => new WalletReportLine() { AccountId = s.Key, Amount = s.Sum(r => r.Amount) })
                    .ToArray();

            _logger.LogDebug("The balance is calculated");
            tran.Complete();

            return report;
        }
    }
}
