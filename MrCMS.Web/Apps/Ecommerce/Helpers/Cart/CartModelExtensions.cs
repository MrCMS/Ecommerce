using System;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Models.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Helpers.Cart
{
    public static class CartModelExtensions
    {
        public static string CartPageShipping(this CartModel cart)
        {
            switch (cart.ShippingStatus)
            {
                case CartShippingStatus.ShippingNotRequired:
                    return "Not required";
                case CartShippingStatus.CannotShip:
                    return "Cannot ship";
                case CartShippingStatus.ShippingNotSet:
                    IOrderedEnumerable<ShippingAmount> shippingAmounts =
                        cart.PotentiallyAvailableShippingMethods.Select(method => method.GetShippingTotal(cart))
                            .OrderBy(amount => amount.Value);
                    return string.Format("From {0}", shippingAmounts.First().Value.ToCurrencyFormat());
                case CartShippingStatus.ShippingSet:
                    return cart.ShippingMethod.GetShippingTotal(cart).Value.ToCurrencyFormat();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static string CartPageTotal(this CartModel cart)
        {
            switch (cart.ShippingStatus)
            {
                case CartShippingStatus.ShippingSet:
                case CartShippingStatus.ShippingNotRequired:
                    return cart.Total.ToCurrencyFormat();
                case CartShippingStatus.CannotShip:
                    return "Cannot complete order";
                case CartShippingStatus.ShippingNotSet:
                    IOrderedEnumerable<ShippingAmount> shippingAmounts =
                        cart.PotentiallyAvailableShippingMethods.Select(method => method.GetShippingTotal(cart))
                            .OrderBy(amount => amount.Value);
                    decimal value = shippingAmounts.First().Value;
                    return string.Format("From {0}", (cart.TotalPreShipping + value).ToCurrencyFormat());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}