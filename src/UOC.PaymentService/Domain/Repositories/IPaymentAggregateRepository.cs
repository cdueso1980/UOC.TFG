using System.Threading.Tasks;

namespace UOC.PaymentService.Domain
{
    public interface IPaymentAggregateRepository 
    {
        Task Add(Payment payment);
        Task Save();
    }
}