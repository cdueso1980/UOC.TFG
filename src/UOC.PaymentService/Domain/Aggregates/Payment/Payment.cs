using System;

namespace UOC.PaymentService.Domain
{
    public class Payment : AggregateRoot
    {
        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public Guid OrderId { get; private set; }
        public decimal TotalAmount { get; private set; }
        public PaymentStatus Status { get; private set; }

        public Payment() { }

        public Payment(Guid customerId, Guid orderId, decimal totalAmount)
        {
            this.Id = Guid.NewGuid();
            this.CustomerId = customerId;
            this.OrderId = orderId;
            this.TotalAmount = totalAmount;
            this.Status = PaymentStatus.Pending;
        }

        public virtual void ChangeStatusToCompleted()
        {
            if(this.Status != PaymentStatus.Pending)
                throw new DomainException("Can't change status to Payment, because is not currently in Created status");
                
            this.Status = PaymentStatus.Completed;
        }

        public virtual void ChangeStatusToRejected()
        {
            if(this.Status != PaymentStatus.Pending)
                throw new DomainException("Can't change status to Payment, because is not currently in Created status");
                
            this.Status = PaymentStatus.Rejected;
        }
    }
}