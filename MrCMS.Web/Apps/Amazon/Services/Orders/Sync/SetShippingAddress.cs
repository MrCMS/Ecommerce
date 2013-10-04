using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public class SetShippingAddress : IPerformAmazonUpdates
    {
        private readonly IValidateAmazonOrderService _validateAmazonOrderService;

        public SetShippingAddress(IValidateAmazonOrderService validateAmazonOrderService)
        {
            _validateAmazonOrderService = validateAmazonOrderService;
        }

        public void Update(AmazonOrder amazonOrder, Order order)
        {
            if (amazonOrder.Status == AmazonOrderStatus.Unshipped)
            {
                var shippingAddress = _validateAmazonOrderService.GetAmazonOrderAddress(order);
                _validateAmazonOrderService.SetShippingAddress(amazonOrder, shippingAddress);
            }
        }

        public int Order { get; private set; }
    }
}