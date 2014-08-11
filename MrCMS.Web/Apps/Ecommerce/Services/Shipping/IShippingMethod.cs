using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Models.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Services.Shipping
{
    public interface IShippingMethod
    {
        string Name { get; }
        string DisplayName { get; }
        string Description { get; }

        bool CanBeUsed(CartModel cart);
        bool CanPotentiallyBeUsed(CartModel cart);
        ShippingAmount GetShippingTotal(CartModel cart);
        ShippingAmount GetShippingTax(CartModel cart);
        decimal TaxRatePercentage { get; }
        
        string ConfigureAction { get; }
        string ConfigureController { get; }
    }

    public enum ShippingMethodAvailablity
    {
        UnavailableForCart,
        UnavailableForLocation,
        PossiblyAvailablePendingLocation,
        Available
    }
}