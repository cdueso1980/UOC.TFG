using System;
using UOC.OrderService.Domain;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace UOC.OrderService.Infrastructure.Persistence
{
    public sealed class OrderAggregateRepository : IOrderAggregateRepository
    {
        private readonly OrderDbContext dbContext;
        
        public OrderAggregateRepository(OrderDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task Add(Order order)
        {
            await this.dbContext.Set<Order>().AddAsync(order);
        }

        public async Task<Order> Get(Guid id)
        {
            return await dbContext.Set<Order>().Include("lines").FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task Save()
        {
             await this.dbContext.SaveChangesAsync();
        }
    }
}