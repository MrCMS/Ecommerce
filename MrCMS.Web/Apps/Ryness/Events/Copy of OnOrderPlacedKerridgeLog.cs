using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;
using MrCMS.Web.Apps.Ryness.Entities;
using MrCMS.Web.Apps.Ryness.Services;

namespace MrCMS.Web.Apps.Ryness.Events
{
    public class OnOrderPaidKerridgeLog : IOnOrderPaid
    {
        private readonly IKerridgeService _kerridgeService;

        public OnOrderPaidKerridgeLog(IKerridgeService kerridgeService)
        {
            _kerridgeService = kerridgeService;
        }

        public int Order { get { return 100; } }
        public void OnOrderPaid(Order order)
        {
            var kerridgeLog = new KerridgeLog
            {
                Order = order,
                Sent = false
            };
            _kerridgeService.Add(kerridgeLog);
        }
    }
}