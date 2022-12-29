using System;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UOC.Orchestrator.Domain;

namespace UOC.Orchestrator.Infrastructure.Persistence
{
    public class OrderPaymentSagaMap : SagaClassMap<OrderPaymentState>
    {
        protected override void Configure(EntityTypeBuilder<OrderPaymentState> entity, ModelBuilder model)
        {
            entity.ToTable("order_payment_saga", schema: "orchestration");
            entity.Property(x => x.OrderId).HasColumnName("order_id");
            entity.Property(x => x.CorrelationId).HasColumnName("correlation_id");
            entity.Property(x => x.IsPaymentRejected).HasColumnName("is_payment_rejected");
            entity.Property(x => x.State).HasColumnName("state");
            entity.Property<DateTime>("LastTimeUpdated").HasColumnName("last_time_updated");
            entity.UseXminAsConcurrencyToken();
        }
    }
}