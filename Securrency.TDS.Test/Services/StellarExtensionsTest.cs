using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;
using Securrency.TDS.Web.DataLayer.Entities;
using Securrency.TDS.Web.Services.PaymentService;
using stellar_dotnet_sdk.responses;
using stellar_dotnet_sdk.responses.operations;

namespace Securrency.TDS.Test.Services
{
    class StellarExtensionsTest
    {
        [Test]
        public void Test_ToEntity_Works()
        {
            //Arrange
            var jsonResponse =
                "{\"id\": \"3531317815820289\",\r\n\"transaction_successful\": true,\r\n\"source_account\": \"GCPWZSMOLI7SWZWQYILSPZIPJMB3ZOR5JNB6DH2OPAPLPAD2BHMRP2VT\",\r\n\"type\": \"payment\",\r\n\"type_i\": 1,\r\n\"created_at\": \"2021-05-06T09:42:47Z\",\r\n\"transaction_hash\": \"20e99c23d970947fc020f90e9063d6edf92a7736f2c5cab77b64a5c5d9823f8f\",\r\n\"asset_type\": \"native\",\r\n\"from\": \"GCPWZSMOLI7SWZWQYILSPZIPJMB3ZOR5JNB6DH2OPAPLPAD2BHMRP2VT\",\r\n\"to\": \"GCYY337UP2VNQTJ4AZTO2K54HK5V5RARB6BDFQXLFVZMEKFPTZTWBJQP\",\r\n\"amount\": \"10.0000000\"\r\n}";

            //Act
            var textReader = new StringReader(jsonResponse);
            var jsonReader = new JsonTextReader(textReader);
            var deserializer = new OperationDeserializer();
            OperationResponse response = deserializer.ReadJson(jsonReader, typeof(PaymentOperationResponse), null, false, new JsonSerializer());
            var payment = (PaymentOperationResponse) response;
            PaymentEntity entity = payment.ToEntity();
            
            //Assert
            Assert.AreEqual(payment.Id, entity.Id);
            Assert.AreEqual(payment.TransactionSuccessful, entity.TransactionSuccessful);
            Assert.AreEqual(payment.SourceAccount, entity.SourceAccountId);
            Assert.AreEqual(payment.From, entity.From);
            Assert.AreEqual(payment.To, entity.To);
            Assert.AreEqual(payment.Amount, entity.Amount.ToString(CultureInfo.InvariantCulture));
        }
    }
}
