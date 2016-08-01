using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public interface ISetTaxDetails
    {
        void SetOrderLinesTaxes(ref Order order);
        void SetShippingTaxes(ref Order order);
    }
}