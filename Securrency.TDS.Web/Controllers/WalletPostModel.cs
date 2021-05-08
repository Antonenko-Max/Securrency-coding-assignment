using System.ComponentModel.DataAnnotations;

namespace Securrency.TDS.Web.Controllers
{
    public class WalletPostModel
    {
        [MaxLength(56)]
        public string AccountId { get; set; }

        public override string ToString() => AccountId;
    }
}
