using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Securrency.TDS.Web.Controllers;
using Securrency.TDS.Web.DataLayer;
using Securrency.TDS.Web.DataLayer.Entities;
using stellar_dotnet_sdk.responses.operations;

namespace Securrency.TDS.Web.Services.PaymentService
{
    public class PaymentServiceImpl : IPaymentService
    {
        private readonly ILogger _logger;
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IStellarClient _stellarClient;

        public PaymentServiceImpl(
            ILogger<PaymentServiceImpl> logger, 
            IDbContextFactory dbContextFactory, 
            IStellarClient stellarClient)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
            _stellarClient = stellarClient;
        }
        
        public async Task UpdatePaymentsAsync(WalletPostModel[] wallets, CancellationToken ct)
        {
            PaymentEntity[] payments = await GetPaymentsFromStellarAsync(wallets, ct);

            await SavePaymentsAsync(payments, ct);
        }

        private async Task<PaymentEntity[]> GetPaymentsFromStellarAsync(WalletPostModel[] wallets, CancellationToken ct)
        {
            _logger.LogDebug("Trying to fetch operations from Stellar service");

            List<OperationResponse>[] records = await _stellarClient.GetPaymentsInNativeAssetAsync(wallets, ct);
            
            PaymentEntity[] payments = records.SelectMany(d => d.Where(r => r.Type == "payment")
                    .Select(p => (PaymentOperationResponse)p)
                    .Where(a => a.AssetType == "native"))
                .Select(r => r.ToEntity())
                .Distinct()
                .ToArray();

            _logger.LogDebug("{0} distinct payments found", payments.Length);
            
            return payments;
        }

        private async Task SavePaymentsAsync(PaymentEntity[] payments, CancellationToken ct)
        {
            if (!payments.Any()) return;
            
            using TransactionScope tran = Tran.BeginScope(IsolationLevel.Serializable);
            using AppDbContext ctx = _dbContextFactory.CreateContext();

            IEnumerable<long> range = payments.Select(p => p.Id);

            _logger.LogDebug("Trying to fetch existed payments in range {0}", range);
            
            ctx.SelectWithUpdLock = true;
            long[] existingPayments = await ctx.Set<PaymentEntity>()
                .Where(p => range.Contains(p.Id))
                .AsNoTracking()
                .Select(p => p.Id)
                .ToArrayAsync(ct);
            
               
            _logger.LogDebug("{0} payments found", existingPayments.Length);

            PaymentEntity[] paymentsToSave =
                payments.Where(p => !existingPayments
                    .Contains(p.Id)).ToArray();
            
            _logger.LogDebug("Trying to save new payments {0}", (object)paymentsToSave);
            
            ctx.Set<PaymentEntity>().AddRange(paymentsToSave);

            await ctx.SaveChangesAsync(ct);
            tran.Complete();
            _logger.LogDebug("{0} new payments successfully saved", paymentsToSave.Count());
        }
    }
}
