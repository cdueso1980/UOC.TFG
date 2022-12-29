using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UOC.PaymentService.Domain;

namespace UOC.PaymentService.Infrastructure.Persistence
{
    public class CustomerPaymentInfoMap : IEntityTypeConfiguration<CustomerPaymentInfo>
    {
        public void Configure(EntityTypeBuilder<CustomerPaymentInfo> builder)
        {
            builder.ToTable("customer_info_payment", schema: "payment");
            builder.HasKey(c => c.CustomerId);
            builder.Property(c => c.CustomerId).HasColumnName("customer_id");
            builder.Property(c => c.IBAN).HasColumnName("iban");
            builder.Property<DateTime?>("CreateDate").HasColumnName("create_date");
            builder.HasMany<Payment>().WithOne().HasForeignKey(r => r.CustomerId);
        }
    }
}