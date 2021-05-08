using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using NUnit.Framework;
using Securrency.TDS.Web.DataLayer;
using Securrency.TDS.Web.DataLayer.Entities;
using Securrency.TDS.Web.Services.ReportService;

namespace Securrency.TDS.Test.Services
{
    class ReportServiceImplTest
    {
        private ReportServiceImpl _service;
        private AutoMock _mock;

        [SetUp]
        public void SetUp()
        {
            Db.Recreate();
            _mock = Mock.Auto();
            _service = _mock.Create<ReportServiceImpl>();
        }

        [Test]
        public async Task Test_GenerateReportAsync_Functional()
        {
            //Arrange
            var payments = new PaymentEntity[]
            {
                new() {Id = 1, SourceAccountId = "1", TransactionSuccessful = true, From = "1", To = "2", Amount = 10},
                new() {Id = 2, SourceAccountId = "1", TransactionSuccessful = true, From = "1", To = "2", Amount = 15},
                new() {Id = 3, SourceAccountId = "1", TransactionSuccessful = true, From = "1", To = "3", Amount = 10},
                new() {Id = 4, SourceAccountId = "1", TransactionSuccessful = true, From = "2", To = "1", Amount = 10},
                new() {Id = 5, SourceAccountId = "1", TransactionSuccessful = true, From = "2", To = "1", Amount = 10},
                new() {Id = 6, SourceAccountId = "2", TransactionSuccessful = true, From = "2", To = "3", Amount = 10}
            };

            using (AppDbContext ctx1 = Db.GetAdminContext())
            {
                ctx1.Set<PaymentEntity>().AddRange(payments);
                await ctx1.SaveChangesAsync();
            }

            //Act
            WalletReportLine[] balance = await _service.GenerateReportAsync("1", CancellationToken.None);
            
            //Assert
            Assert.AreEqual(-5m, balance.First(a => a.AccountId == "2").Amount);
            Assert.AreEqual(-10m, balance.First(a => a.AccountId == "3").Amount);
        }

        [Test]
        public async Task Test_GenerateReportAsync_Ignores_Failed_Transactions()
        {
            //Arrange
            var payments = new PaymentEntity[]
            {
                new() {Id = 1, SourceAccountId = "1", TransactionSuccessful = false, From = "1", To = "2", Amount = 10},
                new() {Id = 2, SourceAccountId = "1", TransactionSuccessful = false, From = "1", To = "2", Amount = 15},
                new() {Id = 3, SourceAccountId = "1", TransactionSuccessful = true, From = "1", To = "3", Amount = 10},
                new() {Id = 4, SourceAccountId = "1", TransactionSuccessful = false, From = "2", To = "1", Amount = 10},
                new() {Id = 5, SourceAccountId = "1", TransactionSuccessful = true, From = "2", To = "1", Amount = 10},
                new() {Id = 6, SourceAccountId = "2", TransactionSuccessful = true, From = "2", To = "3", Amount = 10}
            };

            using (AppDbContext ctx1 = Db.GetAdminContext())
            {
                ctx1.Set<PaymentEntity>().AddRange(payments);
                await ctx1.SaveChangesAsync();
            }

            //Act
            WalletReportLine[] balance = await _service.GenerateReportAsync("1", CancellationToken.None);

            //Assert
            Assert.AreEqual(10m, balance.First(a => a.AccountId == "2").Amount);
            Assert.AreEqual(-10m, balance.First(a => a.AccountId == "3").Amount);
        }
    }
}
