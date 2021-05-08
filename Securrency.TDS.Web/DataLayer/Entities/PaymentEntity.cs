using System;
using System.Diagnostics;

namespace Securrency.TDS.Web.DataLayer.Entities
{
    [DebuggerDisplay("Id={Id}, SourceAccountId={SourceAccountId}")]

    public class PaymentEntity : IEquatable<PaymentEntity>
    {
        public long Id { get; set; }
        
        public string SourceAccountId { get; set; }
        
        public bool TransactionSuccessful { get; set; }
        
        public string From { get; set; }
        
        public string To { get; set; }
        
        public decimal Amount { get; set; }

        public override string ToString() => Id.ToString();

        public bool Equals(PaymentEntity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PaymentEntity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
