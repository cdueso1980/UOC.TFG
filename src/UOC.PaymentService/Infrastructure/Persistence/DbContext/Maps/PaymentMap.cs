using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UOC.PaymentService.Domain;

namespace UOC.PaymentService.Infrastructure.Persistence
{
    public class PaymentMap : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("payment", schema: "payment");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.CustomerId).HasColumnName("customer_id");
            builder.Property(c => c.OrderId).HasColumnName("order_id");
            builder.Property(c => c.TotalAmount).HasColumnName("total_amount");
            builder.Property(c => c.Status).HasColumnName("status").HasConversion(v => v.ToString(), v => (PaymentStatus)Enum.Parse(typeof(PaymentStatus), v));
            builder.Property<DateTime?>("CreateDate").HasColumnName("create_date");
            builder.UseXminAsConcurrencyToken();
        }
    }
}