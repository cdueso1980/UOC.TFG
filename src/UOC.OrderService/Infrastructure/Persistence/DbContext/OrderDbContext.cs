using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace UOC.OrderService.Infrastructure.Persistence
{
    public sealed class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options): base(options) 
        { 
            base.ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new OrderMap());
            modelBuilder.ApplyConfiguration(new OrderLineMap());
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            base.ChangeTracker.DetectChanges(); 

            var now = DateTime.UtcNow;

            foreach (var entry in base.ChangeTracker.Entries())
            {
                if(entry.State == EntityState.Added || (entry.State == EntityState.Modified && entry.Property("CreateDate").CurrentValue == null))
                {
                    entry.State = EntityState.Added;
                    entry.Property("CreateDate").CurrentValue = now;
                }

                entry.Property("ModificationDate").CurrentValue = now;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}