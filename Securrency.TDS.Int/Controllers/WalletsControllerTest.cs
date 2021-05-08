using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Securrency.TDS.Test;
using Securrency.TDS.Web.Controllers;
using Securrency.TDS.Web.DataLayer;
using Securrency.TDS.Web.DataLayer.Entities;
using Securrency.TDS.WebClient;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Securrency.TDS.Int.Controllers
{
    public class WalletsControllerTest : IntegrationTestBase
    {
        private TdsClient _tdsClient;

        [SetUp]
        public void SetUp()
        {
            Db.Recreate();
            _tdsClient = new TdsClient(Integration.GetTdsHttpClient(), Integration.CreateLogger<TdsClient>());
        }

        [Test]
        public async Task Test_PostAsync_Functional()
        {
            //Arrange
            Integration.StellarBackend.Server
                .Given(Request.Create()
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithBodyFromFile("Resources/OperationResponse.json"));
            
            var wallets = new[]
            {
                new WalletPostModel() {AccountId = "GCYY337UP2VNQTJ4AZTO2K54HK5V5RARB6BDFQXLFVZMEKFPTZTWBJQP"},
                new WalletPostModel() {AccountId = "GCPWZSMOLI7SWZWQYILSPZIPJMB3ZOR5JNB6DH2OPAPLPAD2BHMRP2VT"}
            };

            //Act
            await _tdsClient.UploadWalletsAsync(wallets, CancellationToken.None);

            //Assert
            using AppDbContext ctx2 = Db.GetAdminContext();
                
            PaymentEntity[] savedPayments = await ctx2.Set<PaymentEntity>().OrderBy(e => e.Id).ToArrayAsync();
            
            Assert.AreEqual(1, savedPayments.Length);
            
            Assert.AreEqual(3531317815820289, savedPayments[0].Id);
            Assert.AreEqual("GCPWZSMOLI7SWZWQYILSPZIPJMB3ZOR5JNB6DH2OPAPLPAD2BHMRP2VT", savedPayments[0].SourceAccountId);
            Assert.AreEqual("GCYY337UP2VNQTJ4AZTO2K54HK5V5RARB6BDFQXLFVZMEKFPTZTWBJQP", savedPayments[0].To);
            Assert.AreEqual("GCPWZSMOLI7SWZWQYILSPZIPJMB3ZOR5JNB6DH2OPAPLPAD2BHMRP2VT", savedPayments[0].From);
            Assert.AreEqual(10, savedPayments[0].Amount);
        }

        [Test]
        public void Test_PostAsync_Proxies_Bad_Request_From_Stellar_Service()
        {
            //Arrange
            Integration.StellarBackend.Server
                .Given(Request.Create()
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(400));
            
            var wallets = new[]
            {
                new WalletPostModel() {AccountId = "GCYY337UP2VNQTJ4AZTO2K54HK5V5RARB6BDFQXLFVZMEKFPTZTWBJQP"},
                new WalletPostModel() {AccountId = "GCPWZSMOLI7SWZWQYILSPZIPJMB3ZOR5JNB6DH2OPAPLPAD2BHMRP2VT"}
            };

            //Act
            var ex = Assert.ThrowsAsync<TdsClientException>(() =>
                _tdsClient.UploadWalletsAsync(wallets, CancellationToken.None));
            
            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, ex!.Details.StatusCode);
        }

        [Test]
        public void Test_PostAsync_Returns_Bad_Request_If_Account_Id_Fails_Validation()
        {
            //Arrange
            var wallets = new[]
            {
                new WalletPostModel() {AccountId = "GCYY337UP2VNQTJ4AZTO2K54HK5V5RARB6BDFQXLFVZMEKFPTZTWBJQP_"},
                new WalletPostModel() {AccountId = "GCPWZSMOLI7SWZWQYILSPZIPJMB3ZOR5JNB6DH2OPAPLPAD2BHMRP2VT"}
            };

            //Act
            var ex = Assert.ThrowsAsync<TdsClientException>(() =>
                _tdsClient.UploadWalletsAsync(wallets, CancellationToken.None));
            
            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, ex!.Details.StatusCode);
        }

        [Test]
        public async Task Test_GetReportAsync_Functional()
        {
            var accountId = "GCYY337UP2VNQTJ4AZTO2K54HK5V5RARB6BDFQXLFVZMEKFPTZTWBJQP";
            //Arrange
            var payments = new PaymentEntity[]
            {
                new() {Id = 1, SourceAccountId = "1", TransactionSuccessful = true, From = accountId, To = "1", Amount = 10},
                new() {Id = 2, SourceAccountId = "1", TransactionSuccessful = true, From = "1", To = accountId, Amount = 15},
                new() {Id = 3, SourceAccountId = "3", TransactionSuccessful = true, From = "3", To = accountId, Amount = 5.01m},
            };
            using (AppDbContext ctx1 = Db.GetAdminContext())
            {
                ctx1.Set<PaymentEntity>().AddRange(payments);
                await ctx1.SaveChangesAsync();
            }

            //Act
            string result = await _tdsClient.DownloadReportAsync(accountId, CancellationToken.None);

            //Assert
            Assert.AreEqual("1,5\r\n3,5.01\r\n", result);
        }

        [Test]
        public void Test_GetReportAsync_Returns_Bad_Request_If_AccoundId_Invalid()
        {
            var accountId = "GCYY337UP2VNQTJ4AZTO2K54HK5V5RARB6BDFQXLFVZMEKFPTZTWBJQP__";
            //Arrange

            //Act
            var ex = Assert.ThrowsAsync<TdsClientException>(() =>
                _tdsClient.DownloadReportAsync(accountId, CancellationToken.None));

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, ex!.Details.StatusCode);
        }

        [Test]
        public void Test_GetReportAsync_Returns_NotFound_If_Report_Empty()
        {
            var accountId = "GCYY337UP2VNQTJ4AZTO2K54HK5V5RARB6BDFQXLFVZMEKFPTZTWBJQP";
            //Arrange

            //Act
            var ex = Assert.ThrowsAsync<TdsClientException>(() =>
                _tdsClient.DownloadReportAsync(accountId, CancellationToken.None));

            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, ex!.Details.StatusCode);
        }
    }
}