using System;
using System.Threading.Tasks;

namespace UOC.PaymentService.Domain
{
    public interface ICustomerPaymentInfoAggregateRepository 
    {
        Task<CustomerPaymentInfo> Get(Guid customerId);
    }
}