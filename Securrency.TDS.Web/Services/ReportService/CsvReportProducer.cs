using System;
using System.Linq;
using System.Text;

namespace Securrency.TDS.Web.Services.ReportService
{
    public static class CsvReportProducer
    {
        public static byte[] ToCsvReport(this WalletReportLine[] balance)
        {
            if (balance == null || !balance.Any()) return Array.Empty<byte>();
            var text = new StringBuilder();
            foreach (WalletReportLine line in balance)
            {
                text.Append(line.AccountId);
                text.Append(",");
                text.Append(line.Amount.ToString("0.#####"));
                text.Append("\r\n");
            }

            return Encoding.ASCII.GetBytes(text.ToString());
        }
    }
}
