using System;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Models;

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
                    IOrderedEnumerable<decimal> shippingAmounts =
                        cart.PotentiallyAvailableShippingMethods.Select(method => method.GetShippingTotal(cart))
                            .OrderBy(amount => amount);
                    return String.Format("From {0}", shippingAmounts.First().ToCurrencyFormat());
                case CartShippingStatus.ShippingSet:
                    return cart.ShippingMethod.GetShippingTotal(cart).ToCurrencyFormat();
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
                    return "Cannot complete order - no shipping options available.";
                case CartShippingStatus.ShippingNotSet:
                    IOrderedEnumerable<decimal> shippingAmounts =
                        cart.PotentiallyAvailableShippingMethods.Select(method => method.GetShippingTotal(cart))
                            .OrderBy(amount => amount);
                    decimal value = shippingAmounts.First();
                    return (cart.TotalPreShipping + value).ToCurrencyFormat();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public static string CartPageTotalToPay(this CartModel cart)
        {
            switch (cart.ShippingStatus)
            {
                case CartShippingStatus.ShippingSet:
                case CartShippingStatus.ShippingNotRequired:
                    return cart.TotalToPay.ToCurrencyFormat();
                case CartShippingStatus.CannotShip:
                    return "Cannot complete order - no shipping options available.";
                case CartShippingStatus.ShippingNotSet:
                    //IOrderedEnumerable<decimal> shippingAmounts =
                    //    cart.PotentiallyAvailableShippingMethods.Select(method => method.GetShippingTotal(cart))
                    //        .OrderBy(amount => amount);
                    //decimal value = shippingAmounts.First();
                    return "Calculated during checkout"; //string.Format("From {0}", (cart.TotalPreShipping + value).ToCurrencyFormat());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static bool AnythingToPay(this CartModel cart)
        {
            return cart.TotalToPay > Decimal.Zero;
        }

        public static bool HasOrderTotalDiscount(this CartModel cartModel)
        {
            return cartModel.OrderTotalDiscount >= Decimal.Zero;
        }

        public static bool HasShippingDiscount(this CartModel cartModel)
        {
            return cartModel.ShippingDiscount >= Decimal.Zero;
        }
    }
}