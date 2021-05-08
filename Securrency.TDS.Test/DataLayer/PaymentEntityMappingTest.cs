using NUnit.Framework;
using Securrency.TDS.Web.DataLayer;
using Securrency.TDS.Web.DataLayer.Entities;

namespace Securrency.TDS.Test.DataLayer
{
    class PaymentEntityMappingTest
    {
        [Test]
        public void Test_Mapping_Functional()
        {
            //Arrange
            Db.Recreate();

            var payment = new PaymentEntity()
                {Id = 1, SourceAccountId = "1", TransactionSuccessful = true, Amount = 1, From = "1", To = "2"};

            //Act
            using (AppDbContext ctx1 = Db.GetAdminContext())
            {
                ctx1.Set<PaymentEntity>().Add(payment);
                ctx1.SaveChanges();
            }

            //Assert
            using AppDbContext ctx2 = Db.GetAdminContext();

            PaymentEntity savedPayment = ctx2.Set<PaymentEntity>().Find(payment.Id);

            Assert.AreEqual(payment.Id, savedPayment.Id);
            Assert.AreEqual(payment.SourceAccountId, savedPayment.SourceAccountId);
            Assert.AreEqual(payment.To, savedPayment.To);
            Assert.AreEqual(payment.From, savedPayment.From);
            Assert.AreEqual(payment.Amount, savedPayment.Amount);
            Assert.AreEqual(payment.TransactionSuccessful, savedPayment.TransactionSuccessful);
        }
    }
}
