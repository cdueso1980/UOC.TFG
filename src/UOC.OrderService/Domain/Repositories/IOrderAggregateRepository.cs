using System;
using System.Threading.Tasks;

namespace UOC.OrderService.Domain
{
    public interface IOrderAggregateRepository
    {
       Task<Order> Get(Guid id);
       Task Add(Order order);
       Task Save();
    }
}