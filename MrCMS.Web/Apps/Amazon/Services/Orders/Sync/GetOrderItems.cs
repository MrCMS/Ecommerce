using System.Linq;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Services.Api.Orders;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public class GetOrderItems : IPerformAmazonUpdates
    {
        private readonly IAmazonOrdersApiService _amazonOrdersApiService;
        private readonly IImportAmazonOrderService _importAmazonOrderService;

        public GetOrderItems(IAmazonOrdersApiService amazonOrdersApiService, IImportAmazonOrderService importAmazonOrderService)
        {
            _amazonOrdersApiService = amazonOrdersApiService;
            _importAmazonOrderService = importAmazonOrderService;
        }

        public void Update(AmazonOrder amazonOrder, Order order)
        {
            if (!amazonOrder.Items.Any())
            {
                var orderItems = _amazonOrdersApiService.ListOrderItems(amazonOrder.AmazonOrderId);
                _importAmazonOrderService.SetAmazonOrderItems(orderItems, amazonOrder);
            }
        }

        public int Order { get { return -5; } }
    }
}