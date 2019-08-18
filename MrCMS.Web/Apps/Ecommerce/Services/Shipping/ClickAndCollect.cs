using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Helpers.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Shipping
{
    public class ClickAndCollect : IShippingMethod
    {
        public string Name => "Click and Collect";
        public string DisplayName => "Click and Collect";
        public string Description => "Click and Collect";
        public string TypeName => GetType().FullName;
        public bool CanBeUsed(CartModel cart)
        {
            return CanPotentiallyBeUsed(cart);
        }

        public bool CanPotentiallyBeUsed(CartModel cart)
        {
            if (cart == null || cart.Items.Any(item => !item.IsAbleToUseShippingMethod(this)))
                return false;

            return true;
        }

        public decimal GetShippingTotal(CartModel cart)
        {
            return 0m;
        }

        public decimal GetShippingTax(CartModel cart)
        {
            return 0m;
        }

        public decimal TaxRatePercentage => 0;
        public string ConfigureController => "ClickAndCollect";

        public string ConfigureAction => "Configure";
    }
}