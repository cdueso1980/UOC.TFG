using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace UOC.PaymentService.Infrastructure.Persistence
{
    public sealed class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options): base(options) 
        { 
            base.ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new PaymentMap());
            modelBuilder.ApplyConfiguration(new CustomerPaymentInfoMap());
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            base.ChangeTracker.DetectChanges(); 

            var now = DateTime.UtcNow;

            foreach (var entry in base.ChangeTracker.Entries())
            {
                if(entry.State == EntityState.Modified && entry.Property("CreateDate").CurrentValue == null)
                {
                    entry.State = EntityState.Added;
                }

                entry.Property("CreateDate").CurrentValue = now;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}