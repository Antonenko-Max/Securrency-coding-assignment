using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUnit.Framework;
using Securrency.TDS.Web.Controllers;
using Securrency.TDS.Web.DataLayer;
using Securrency.TDS.Web.DataLayer.Entities;
using Securrency.TDS.Web.Services.PaymentService;
using stellar_dotnet_sdk.responses;
using stellar_dotnet_sdk.responses.operations;

namespace Securrency.TDS.Test.Services
{
    class PaymentServiceImplTest
    {
        private PaymentServiceImpl _service;
        private AutoMock _mock;

        [SetUp]
        public void SetUp()
        {
            Db.Recreate();
            _mock = Mock.Auto();
            _service = _mock.Create<PaymentServiceImpl>();
        }

        [Test]
        public async Task UpdatePaymentsAsync_Saves_New_Payments()
        {
            //Arrange
            var jsonResponse1 =
                "{\"id\": \"3531317815820289\",\r\n\"transaction_successful\": true,\r\n\"source_account\": \"GCPWZSMOLI7SWZWQYILSPZIPJMB3ZOR5JNB6DH2OPAPLPAD2BHMRP2VT\",\r\n\"type\": \"payment\",\r\n\"type_i\": 1,\r\n\"created_at\": \"2021-05-06T09:42:47Z\",\r\n\"transaction_hash\": \"20e99c23d970947fc020f90e9063d6edf92a7736f2c5cab77b64a5c5d9823f8f\",\r\n\"asset_type\": \"native\",\r\n\"from\": \"GCPWZSMOLI7SWZWQYILSPZIPJMB3ZOR5JNB6DH2OPAPLPAD2BHMRP2VT\",\r\n\"to\": \"GCYY337UP2VNQTJ4AZTO2K54HK5V5RARB6BDFQXLFVZMEKFPTZTWBJQP\",\r\n\"amount\": \"10.0000000\"\r\n}";
            var jsonResponse2 =
                "{\"id\": \"3531317815820290\",\r\n\"transaction_successful\": true,\r\n\"source_account\": \"GCPWZSMOLI7SWZWQYILSPZIPJMB3ZOR5JNB6DH2OPAPLPAD2BHMRP2VT\",\r\n\"type\": \"payment\",\r\n\"type_i\": 1,\r\n\"created_at\": \"2021-05-06T09:42:47Z\",\r\n\"transaction_hash\": \"20e99c23d970947fc020f90e9063d6edf92a7736f2c5cab77b64a5c5d9823f8f\",\r\n\"asset_type\": \"native\",\r\n\"from\": \"GCPWZSMOLI7SWZWQYILSPZIPJMB3ZOR5JNB6DH2OPAPLPAD2BHMRP2VT\",\r\n\"to\": \"GCYY337UP2VNQTJ4AZTO2K54HK5V5RARB6BDFQXLFVZMEKFPTZTWBJQP\",\r\n\"amount\": \"10.0000000\"\r\n}";

            var deserializer = new OperationDeserializer();
            OperationResponse response1 = deserializer.ReadJson(new JsonTextReader(new StringReader(jsonResponse1)),
                typeof(PaymentOperationResponse), null, false, new JsonSerializer());
            OperationResponse response2 = deserializer.ReadJson(new JsonTextReader(new StringReader(jsonResponse2)),
                typeof(PaymentOperationResponse), null, false, new JsonSerializer());

            WalletPostModel[] wallets = Array.Empty<WalletPostModel>();
            var operations = new List<OperationResponse>[]
            {
                new() {response1, response2}
            };

            PaymentEntity payment1 = ((PaymentOperationResponse)response1).ToEntity();
            PaymentEntity payment2 = ((PaymentOperationResponse)response2).ToEntity();

            _mock.Mock<IStellarClient>()
                .Setup(s => s.GetPaymentsInNativeAssetAsync(wallets, CancellationToken.None))
                .Returns(Task.FromResult(operations));

            //Act
            await _service.UpdatePaymentsAsync(wallets, CancellationToken.None);

            //Assert
            using AppDbContext ctx2 = Db.GetAdminContext();

            PaymentEntity savedPayment1 = await ctx2.Set<PaymentEntity>().FindAsync(payment1.Id);

            Assert.AreEqual(payment1.Id, savedPayment1.Id);
            Assert.AreEqual(payment1.SourceAccountId, savedPayment1.SourceAccountId);
            Assert.AreEqual(payment1.To, savedPayment1.To);
            Assert.AreEqual(payment1.From, savedPayment1.From);
            Assert.AreEqual(payment1.Amount, savedPayment1.Amount);
            Assert.AreEqual(payment1.TransactionSuccessful, savedPayment1.TransactionSuccessful);

            PaymentEntity savedPayment2 = await ctx2.Set<PaymentEntity>().FindAsync(payment2.Id);

            Assert.AreEqual(payment2.Id, savedPayment2.Id);
            Assert.AreEqual(payment2.SourceAccountId, savedPayment2.SourceAccountId);
            Assert.AreEqual(payment2.To, savedPayment2.To);
            Assert.AreEqual(payment2.From, savedPayment2.From);
            Assert.AreEqual(payment2.Amount, savedPayment2.Amount);
            Assert.AreEqual(payment2.TransactionSuccessful, savedPayment2.TransactionSuccessful);

        }

        [Test]
        public async Task UpdatePaymentsAsync_Does_Not_Duplicate_Existing_Payment()
        {
            //Arrange
            var jsonResponse1 =
                "{\"id\": \"3531317815820289\",\r\n\"transaction_successful\": true,\r\n\"source_account\": \"GCPWZSMOLI7SWZWQYILSPZIPJMB3ZOR5JNB6DH2OPAPLPAD2BHMRP2VT\",\r\n\"type\": \"payment\",\r\n\"type_i\": 1,\r\n\"created_at\": \"2021-05-06T09:42:47Z\",\r\n\"transaction_hash\": \"20e99c23d970947fc020f90e9063d6edf92a7736f2c5cab77b64a5c5d9823f8f\",\r\n\"asset_type\": \"native\",\r\n\"from\": \"GCPWZSMOLI7SWZWQYILSPZIPJMB3ZOR5JNB6DH2OPAPLPAD2BHMRP2VT\",\r\n\"to\": \"GCYY337UP2VNQTJ4AZTO2K54HK5V5RARB6BDFQXLFVZMEKFPTZTWBJQP\",\r\n\"amount\": \"10.0000000\"\r\n}";

            var deserializer = new OperationDeserializer();
            OperationResponse response1 = deserializer.ReadJson(new JsonTextReader(new StringReader(jsonResponse1)), typeof(PaymentOperationResponse), null, false, new JsonSerializer());

            WalletPostModel[] wallets = Array.Empty<WalletPostModel>();
            var operations = new List<OperationResponse>[]
            {
                new() {response1},
            };

            PaymentEntity payment = ((PaymentOperationResponse) response1).ToEntity();
            _mock.Mock<IStellarClient>()
                .Setup(s => s.GetPaymentsInNativeAssetAsync(wallets, CancellationToken.None))
                .Returns(Task.FromResult(operations));

            using (AppDbContext ctx1 = Db.GetAdminContext())
            {
                ctx1.Set<PaymentEntity>().Add(payment);
                await ctx1.SaveChangesAsync();
            }

            //Act
            await _service.UpdatePaymentsAsync(wallets, CancellationToken.None);

            //Assert
            using AppDbContext ctx2 = Db.GetAdminContext();

            PaymentEntity[] savedPayments = await ctx2.Set<PaymentEntity>().OrderBy(e => e.Id).ToArrayAsync();

            Assert.AreEqual(1, savedPayments.Length);
        }

        [Test]
        public async Task UpdatePaymentsAsync_Removes_Identical_Payments_Before_Save()
        {
            //Arrange
            var jsonResponse1 =
                "{\"id\": \"3531317815820289\",\r\n\"transaction_successful\": true,\r\n\"source_account\": \"GCPWZSMOLI7SWZWQYILSPZIPJMB3ZOR5JNB6DH2OPAPLPAD2BHMRP2VT\",\r\n\"type\": \"payment\",\r\n\"type_i\": 1,\r\n\"created_at\": \"2021-05-06T09:42:47Z\",\r\n\"transaction_hash\": \"20e99c23d970947fc020f90e9063d6edf92a7736f2c5cab77b64a5c5d9823f8f\",\r\n\"asset_type\": \"native\",\r\n\"from\": \"GCPWZSMOLI7SWZWQYILSPZIPJMB3ZOR5JNB6DH2OPAPLPAD2BHMRP2VT\",\r\n\"to\": \"GCYY337UP2VNQTJ4AZTO2K54HK5V5RARB6BDFQXLFVZMEKFPTZTWBJQP\",\r\n\"amount\": \"10.0000000\"\r\n}";
            var jsonResponse2 =
                "{\"id\": \"3531317815820289\",\r\n\"transaction_successful\": true,\r\n\"source_account\": \"GCPWZSMOLI7SWZWQYILSPZIPJMB3ZOR5JNB6DH2OPAPLPAD2BHMRP2VT\",\r\n\"type\": \"payment\",\r\n\"type_i\": 1,\r\n\"created_at\": \"2021-05-06T09:42:47Z\",\r\n\"transaction_hash\": \"20e99c23d970947fc020f90e9063d6edf92a7736f2c5cab77b64a5c5d9823f8f\",\r\n\"asset_type\": \"native\",\r\n\"from\": \"GCPWZSMOLI7SWZWQYILSPZIPJMB3ZOR5JNB6DH2OPAPLPAD2BHMRP2VT\",\r\n\"to\": \"GCYY337UP2VNQTJ4AZTO2K54HK5V5RARB6BDFQXLFVZMEKFPTZTWBJQP\",\r\n\"amount\": \"10.0000000\"\r\n}";

            var deserializer = new OperationDeserializer();
            OperationResponse response1 = deserializer.ReadJson(new JsonTextReader(new StringReader(jsonResponse1)), typeof(PaymentOperationResponse), null, false, new JsonSerializer());
            OperationResponse response2 = deserializer.ReadJson(new JsonTextReader(new StringReader(jsonResponse2)), typeof(PaymentOperationResponse), null, false, new JsonSerializer());

            WalletPostModel[] wallets = Array.Empty<WalletPostModel>();
            var operations = new List<OperationResponse>[]
            {
                new() {response1},
                new() {response2}
            };

            _mock.Mock<IStellarClient>()
                .Setup(s => s.GetPaymentsInNativeAssetAsync(wallets, CancellationToken.None))
                .Returns(Task.FromResult(operations));


            //Act
            await _service.UpdatePaymentsAsync(wallets, CancellationToken.None);

            //Assert
            using AppDbContext ctx2 = Db.GetAdminContext();

            PaymentEntity[] savedPayments = await ctx2.Set<PaymentEntity>().OrderBy(e => e.Id).ToArrayAsync();

            Assert.AreEqual(1, savedPayments.Length);

        }

        [Test]
        public async Task UpdatePaymentsAsync_Filters_Payment_Operations()
        {
            //Arrange - create account operation
            var jsonResponse1 =
                "{\"id\": \"3488471222067201\",\r\n\"paging_token\": \"3488471222067201\",\r\n\"transaction_successful\": true,\r\n\"source_account\": \"GAIH3ULLFQ4DGSECF2AR555KZ4KNDGEKN4AFI4SU2M7B43MGK3QJZNSR\",\r\n\"type\": \"create_account\",\r\n\"type_i\": 0,\r\n\"created_at\": \"2021-05-05T19:08:42Z\",\r\n\"transaction_hash\": \"3b0e2c42576d0bfb9fab6b07fb26a8d992561d7199e41aa7cc0f8889e5b87521\",\r\n\"starting_balance\": \"10000.0000000\",\r\n\"funder\": \"GAIH3ULLFQ4DGSECF2AR555KZ4KNDGEKN4AFI4SU2M7B43MGK3QJZNSR\",\r\n\"account\": \"GCYY337UP2VNQTJ4AZTO2K54HK5V5RARB6BDFQXLFVZMEKFPTZTWBJQP\"\r\n}";

            var deserializer = new OperationDeserializer();
            OperationResponse response1 = deserializer.ReadJson(new JsonTextReader(new StringReader(jsonResponse1)),
                typeof(PaymentOperationResponse), null, false, new JsonSerializer());

            WalletPostModel[] wallets = Array.Empty<WalletPostModel>();
            var operations = new List<OperationResponse>[]
            {
                new() {response1}
            };

            _mock.Mock<IStellarClient>()
                .Setup(s => s.GetPaymentsInNativeAssetAsync(wallets, CancellationToken.None))
                .Returns(Task.FromResult(operations));

            //Act
            await _service.UpdatePaymentsAsync(wallets, CancellationToken.None);

            //Assert
            using AppDbContext ctx2 = Db.GetAdminContext();

            PaymentEntity[] savedPayments = await ctx2.Set<PaymentEntity>().ToArrayAsync();
            
            CollectionAssert.IsEmpty(savedPayments);
        }

        [Test]
        public async Task UpdatePaymentsAsync_Filters_Payment_Operations_With_Native_Asset()
        {
            //Arrange - create account operation
            var jsonResponse1 =
                "{\"id\": \"3531317815820289\",\r\n\"transaction_successful\": true,\r\n\"source_account\": \"GCPWZSMOLI7SWZWQYILSPZIPJMB3ZOR5JNB6DH2OPAPLPAD2BHMRP2VT\",\r\n\"type\": \"payment\",\r\n\"type_i\": 1,\r\n\"created_at\": \"2021-05-06T09:42:47Z\",\r\n\"transaction_hash\": \"20e99c23d970947fc020f90e9063d6edf92a7736f2c5cab77b64a5c5d9823f8f\",\r\n\"asset_type\": \"USD\",\r\n\"from\": \"GCPWZSMOLI7SWZWQYILSPZIPJMB3ZOR5JNB6DH2OPAPLPAD2BHMRP2VT\",\r\n\"to\": \"GCYY337UP2VNQTJ4AZTO2K54HK5V5RARB6BDFQXLFVZMEKFPTZTWBJQP\",\r\n\"amount\": \"10.0000000\"\r\n}";

            var deserializer = new OperationDeserializer();
            OperationResponse response1 = deserializer.ReadJson(new JsonTextReader(new StringReader(jsonResponse1)),
                typeof(PaymentOperationResponse), null, false, new JsonSerializer());

            WalletPostModel[] wallets = Array.Empty<WalletPostModel>();
            var operations = new List<OperationResponse>[]
            {
                new() {response1}
            };

            _mock.Mock<IStellarClient>()
                .Setup(s => s.GetPaymentsInNativeAssetAsync(wallets, CancellationToken.None))
                .Returns(Task.FromResult(operations));

            //Act
            await _service.UpdatePaymentsAsync(wallets, CancellationToken.None);

            //Assert
            using AppDbContext ctx2 = Db.GetAdminContext();

            PaymentEntity[] savedPayments = await ctx2.Set<PaymentEntity>().ToArrayAsync();

            CollectionAssert.IsEmpty(savedPayments);
        }
    }
}
