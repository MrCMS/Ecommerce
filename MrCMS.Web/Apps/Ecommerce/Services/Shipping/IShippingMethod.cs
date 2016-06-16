using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Shipping
{
    public interface IShippingMethod
    {
        string Name { get; }
        string DisplayName { get; }
        string Description { get; }

        string TypeName { get; }
        bool CanBeUsed(CartModel cart);
        bool CanPotentiallyBeUsed(CartModel cart);
        decimal GetShippingTotal(CartModel cart);
        decimal GetShippingTax(CartModel cart);
        decimal TaxRatePercentage { get; }
        
        string ConfigureAction { get; }
        string ConfigureController { get; }
    }
}