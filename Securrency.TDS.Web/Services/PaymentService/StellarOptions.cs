using System.ComponentModel.DataAnnotations;

namespace Securrency.TDS.Web.Services.StellarService
{
    public class StellarOptions
    {
        [Required]
        public string BaseAddress { get; set; }
    }
}
