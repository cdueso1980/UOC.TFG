using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UOC.OrderService.Domain;

namespace UOC.OrderService.Infrastructure.Persistence
{
    public class OrderLineMap : IEntityTypeConfiguration<OrderLine>
    {
        public void Configure(EntityTypeBuilder<OrderLine> builder)
        {
            builder.ToTable("order_line", schema: "order");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.ProductId).HasColumnName("product_id");
            builder.Property(c => c.Amount).HasColumnName("amount");
            builder.Property<Guid>("OrderId").HasColumnName("order_id");
            builder.Property<DateTime?>("CreateDate").HasColumnName("create_date");
            builder.Property<DateTime?>("ModificationDate").HasColumnName("modification_date");
        }
    }
}