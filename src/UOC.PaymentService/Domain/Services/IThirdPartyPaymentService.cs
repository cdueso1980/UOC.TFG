using System.Threading.Tasks;

namespace UOC.PaymentService.Domain
{
    public interface IThirdPartyPaymentService 
    {
        Task<bool> SendPay(ThirdPartyPaymentRequest request);
    }
}