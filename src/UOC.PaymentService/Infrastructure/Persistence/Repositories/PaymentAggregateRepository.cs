using System;
using System.Threading.Tasks;
using UOC.PaymentService.Domain;

namespace UOC.PaymentService.Infrastructure.Persistence
{
    public sealed class PaymentAggregateRepository : IPaymentAggregateRepository
    {
        private readonly PaymentDbContext dbContext;
        
        public PaymentAggregateRepository(PaymentDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task Add(Payment payment)
        {
            await this.dbContext.Set<Payment>().AddAsync(payment);
        }

        public async Task Save()
        {
             await this.dbContext.SaveChangesAsync();
        }
    }
}