using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using Order = MrCMS.Web.Apps.Ecommerce.Entities.Orders.Order;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public interface IValidateAmazonOrderService
    {
        void SetAmazonOrderItem(ref AmazonOrder amazonOrder, OrderItem rawOrderItem);
        void GetAmazonOrderDetails(MarketplaceWebServiceOrders.Model.Order rawOrder, ref AmazonOrder order, AddressData shippingAddress);
        void SetShippingAddress(AmazonOrder amazonOrder, AddressData address);
        AddressData GetAmazonOrderAddress(MarketplaceWebServiceOrders.Model.Order rawOrder);
        Order GetOrder(AmazonOrder amazonOrder);
    }
}