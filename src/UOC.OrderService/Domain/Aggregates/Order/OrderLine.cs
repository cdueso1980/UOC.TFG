using System;

namespace UOC.OrderService.Domain
{
    public class OrderLine
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public decimal Amount { get; private set; }

        public OrderLine() { }

        public OrderLine(Guid productId, decimal amount)
        {
            this.Id = Guid.NewGuid();
            this.ProductId = productId;
            this.Amount = amount;
        }
    }
}