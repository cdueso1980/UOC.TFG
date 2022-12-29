using System;

namespace UOC.PaymentService.Domain
{
    public record ThirdPartyPaymentRequest 
    {
        public Guid OrderId { get; init; }
        public string IBAN { get; init; }
        public decimal TotalAmount { get; init; }
    }
} 