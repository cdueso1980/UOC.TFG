using UOC.PaymentService.Domain;
using System.Threading.Tasks;
using System;

namespace UOC.PaymentService.Infrastructure.Domain.Services
{
    public sealed class ThirdPartyPaymentService : IThirdPartyPaymentService
    {
        public Task<bool> SendPay(ThirdPartyPaymentRequest request)
        {
            //Note: simulate real call to externbal service
            if(request.OrderId == Guid.Parse("49aeef10-bd31-4431-a1b9-03927681ec05"))
                return Task.FromResult(false);

            return Task.FromResult(true);
        }
    }
}