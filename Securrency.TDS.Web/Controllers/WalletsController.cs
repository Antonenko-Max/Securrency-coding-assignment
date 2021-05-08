using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Securrency.TDS.Web.Services.PaymentService;

namespace Securrency.TDS.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger _logger;

        public WalletsController(
            IPaymentService paymentService, 
            ILogger<WalletsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
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
    }
}
