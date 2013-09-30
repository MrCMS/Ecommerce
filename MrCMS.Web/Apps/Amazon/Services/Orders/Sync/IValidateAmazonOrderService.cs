using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using Address = MrCMS.Web.Apps.Ecommerce.Entities.Users.Address;
using Order = MrCMS.Web.Apps.Ecommerce.Entities.Orders.Order;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public interface IValidateAmazonOrderService
    {
        void SetAmazonOrderItem(ref AmazonOrder amazonOrder, OrderItem rawOrderItem);
        void GetAmazonOrderDetails(MarketplaceWebServiceOrders.Model.Order rawOrder, ref AmazonOrder order,Address shippingAddress);
        Address GetAmazonOrderAddress(MarketplaceWebServiceOrders.Model.Order rawOrder);
        Order GetOrder(MarketplaceWebServiceOrders.Model.Order rawOrder, AmazonOrder amazonOrder);
    }
}