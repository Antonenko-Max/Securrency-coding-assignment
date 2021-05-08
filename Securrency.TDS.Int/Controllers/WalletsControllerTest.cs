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
            await _tdsClient.UploadWallets(wallets, CancellationToken.None);

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
                _tdsClient.UploadWallets(wallets, CancellationToken.None));
            
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
                _tdsClient.UploadWallets(wallets, CancellationToken.None));
            
            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, ex!.Details.StatusCode);
        }
    }
}