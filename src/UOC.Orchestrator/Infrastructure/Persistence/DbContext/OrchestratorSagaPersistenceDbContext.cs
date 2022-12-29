using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

namespace UOC.Orchestrator.Infrastructure.Persistence
{
    public class OrchestratorSagaPersistenceDbContext :  SagaDbContext
    {
        public OrchestratorSagaPersistenceDbContext(DbContextOptions options) : base(options){ }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new OrderPaymentSagaMap(); }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entityEntry in entries)
            {
                entityEntry.Property("LastTimeUpdated").CurrentValue = DateTime.UtcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    entityEntry.Property("LastTimeUpdated").CurrentValue = DateTime.UtcNow;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}

