using System;

namespace UOC.PaymentService.Domain
{
    public class CustomerPaymentInfo : AggregateRoot
    {
        public virtual Guid CustomerId { get; private set; }
        public virtual string IBAN { get; private set; }

        public CustomerPaymentInfo() { }

        public CustomerPaymentInfo(Guid customerId, string iban)
        {
            this.CustomerId = customerId;
            this.IBAN = iban;
        }
    }
}