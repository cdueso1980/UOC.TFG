using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UOC.PaymentService.Domain;

namespace UOC.PaymentService.Infrastructure.Persistence
{
    public sealed class CustomerPaymentInfoAggregateRepository : ICustomerPaymentInfoAggregateRepository
    {
        private readonly PaymentDbContext dbContext;
        
        public CustomerPaymentInfoAggregateRepository(PaymentDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<CustomerPaymentInfo> Get(Guid customerId)
        {
            return await this.dbContext.Set<CustomerPaymentInfo>().FirstOrDefaultAsync(r => r.CustomerId == customerId);
        }
    }
}