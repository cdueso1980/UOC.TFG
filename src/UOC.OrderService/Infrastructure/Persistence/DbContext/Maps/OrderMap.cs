using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UOC.OrderService.Domain;

namespace UOC.OrderService.Infrastructure.Persistence
{
    public class OrderMap : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("order", schema: "order");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.CustomerId).HasColumnName("customer_id");
            builder.Property(c => c.Status).HasColumnName("status").HasConversion(v => v.ToString(), v => (OrderStatus)Enum.Parse(typeof(OrderStatus), v));
            builder.Ignore(c => c.Lines);
            builder.HasMany<OrderLine>("lines").WithOne().HasForeignKey("OrderId");
            builder.Property<DateTime?>("CreateDate").HasColumnName("create_date");
            builder.Property<DateTime?>("ModificationDate").HasColumnName("modification_date");
            builder.UseXminAsConcurrencyToken();
        }
    }
}

