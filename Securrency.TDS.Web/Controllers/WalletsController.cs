using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Securrency.TDS.Web.Services.PaymentService;
using Securrency.TDS.Web.Services.ReportService;

namespace Securrency.TDS.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger _logger;
        private readonly IReportService _reportService;

        public WalletsController(
            IPaymentService paymentService, 
            ILogger<WalletsController> logger, 
            IReportService reportService)
        {
            _paymentService = paymentService;
            _logger = logger;
            _reportService = reportService;
        }
        
        /// <summary>
        /// Uploads a list of Stellar Wallet addresses
        /// </summary>
        /// <param name="wallets">the list of wallets</param>
        /// <param name="ct">Cancellation token</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync([FromBody] WalletPostModel[] wallets, CancellationToken ct)
        {
            _logger.LogDebug("Uploading the list of wallets: {0}", (object) wallets);
            try
            {
                await _paymentService.UpdatePaymentsAsync(wallets, ct);
                _logger.LogDebug("The list of wallets uploaded");
                return NoContent();
            }
            catch (ApplicationException e)
            {
                _logger.LogDebug("Upload failed: {0}", e.Message);
                return ValidationProblem(e.Message);
            }
        }

        /// <summary>
        /// Download a csv report for the wallet
        /// </summary>
        /// <param name="accountId">the wallet account Id</param>
        /// <param name="ct">Cancellation token</param>
        [HttpGet("report/{accountId}")]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReportAsync(string accountId, CancellationToken ct)
        {
            if (accountId.Length != 56) return ValidationProblem("Invalid account Id length");

            _logger.LogDebug("Generating a csv report for wallet: {0}", accountId);

            WalletReportLine[] report = await _reportService.GenerateReportAsync(accountId, ct);

            if (!report.Any()) return NotFound();

            _logger.LogDebug("The csv report generated");

            Response.Headers["Content-Disposition"] = "attachment";
            return File(report.ToCsvReport(), "text/csv", "report.csv");
        }

    }
}
