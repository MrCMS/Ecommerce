using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Models.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Services.Shipping
{
    public interface IShippingMethod
    {
        bool CanBeUsed(CartModel cart);
        ShippingAmount GetShippingTotal(CartModel cart);
        ShippingAmount GetShippingTax(CartModel cart);
        string Name { get; }
        string Description { get; }
        decimal TaxRatePercentage { get; }
    }
}