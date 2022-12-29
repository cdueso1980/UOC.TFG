using System;
using System.Linq;
using System.Collections.Generic;

namespace UOC.OrderService.Domain
{
    public class Order : AggregateRoot
    {
        private List<OrderLine> lines { get; set;} = new();
        public virtual Guid CustomerId { get; private set; }
        public virtual OrderStatus Status { get; private set; }
        public IReadOnlyList<OrderLine> Lines => lines;

        public Order() { }

        public Order(Guid customerId, List<(Guid ProductId, Decimal Amount)> OrderLines)
        {
            this.Id = Guid.NewGuid();
            this.CustomerId = customerId;
            this.lines.AddRange(OrderLines.Select(r => new OrderLine(r.ProductId, r.Amount)));
        }

        public virtual void ChangeStatusToPayment()
        {
            if(this.Status != OrderStatus.Created)
                throw new DomainException("Can't change status to Payment, because is not currently in Created status");
                
            this.Status = OrderStatus.Payment;
        }

        public virtual void ChangeStatusToCompleted()
        {
            if(this.Status != OrderStatus.Payment)
                throw new DomainException("Can't change status to Payment, because is not currently in Payment status");
                
            this.Status = OrderStatus.Completed;
        }

        public virtual void ChangeStatusToPaymentRejected()
        {
            if(this.Status != OrderStatus.Payment)
                throw new DomainException("Can't change status to Payment, because is not currently in Payment status");
                
            this.Status = OrderStatus.PaymentRejected;
        }

        public virtual decimal GetTotalAmount()
        {
            return this.Lines.Sum(r => r.Amount);
        }
    }
}