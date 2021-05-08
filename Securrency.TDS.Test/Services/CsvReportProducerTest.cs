using System;
using System.Text;
using NUnit.Framework;
using Securrency.TDS.Web.Services.ReportService;

namespace Securrency.TDS.Test.Services
{
    class CsvReportProducerTest
    {
        [Test]
        public void Test_ToCsvReport_Returns_CsvReport()
        {
            //Arrange
            var balance = new WalletReportLine[]
            {
                new() {AccountId = "1", Amount = 10.01m},
                new() {AccountId = "2", Amount = 20}
            };
            
            //Act
            string report = Encoding.ASCII.GetString(balance.ToCsvReport());
            
            //Assert
            Assert.AreEqual("1,10.01\r\n2,20\r\n", report);
        }

        [Test]
        public void Test_ToCsvReport_Returns_EmptyArray_If_Balance_Is_Empty_Or_Null()
        {
            //Arrange
            WalletReportLine[] balance = Array.Empty<WalletReportLine>();

            //Act
            byte[] report1 = balance.ToCsvReport();
            byte[] report2 = ((WalletReportLine[]) null).ToCsvReport();

            //Assert
            CollectionAssert.IsEmpty(report1);
            CollectionAssert.IsEmpty(report2);
        }
    }
}
