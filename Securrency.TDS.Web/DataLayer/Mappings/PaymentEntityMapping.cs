using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Securrency.TDS.Web.DataLayer.Entities;

namespace Securrency.TDS.Web.DataLayer.Mappings
{
    public class PaymentEntityMapping : IEntityTypeConfiguration<PaymentEntity>
    {
        public void Configure(EntityTypeBuilder<PaymentEntity> builder)
        {
            builder.ToTableInSchema("Payments");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedNever();
            builder.Property(e => e.SourceAccountId).IsRequired().HasMaxLength(60);
            builder.Property(e => e.TransactionSuccessful).IsRequired();
            builder.Property(e => e.From).IsRequired().HasMaxLength(60);
            builder.Property(e => e.To).IsRequired().HasMaxLength(60);
            builder.Property(e => e.Amount).IsRequired().HasColumnType("decimal(15,9)");
        }
    }
}
