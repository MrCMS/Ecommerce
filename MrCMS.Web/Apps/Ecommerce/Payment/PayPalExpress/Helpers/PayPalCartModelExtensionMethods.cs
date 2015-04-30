using System;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress.Helpers
{
    public static class PayPalCartModelExtensionMethods
    {
        public static decimal GetShippingTotalForPayPal(this CartModel cart)
        {
            if (cart.ShippingMethod != null)
                return cart.ShippingTotalPreDiscount;

            var cheapestShippingOption = cart.GetCheapestShippingOption();
            if (cheapestShippingOption == null) return Decimal.Zero;

            return cheapestShippingOption.GetShippingTotal(cart);

        }

        public static IShippingMethod GetCheapestShippingOption(this CartModel cart)
        {
            return cart.PotentiallyAvailableShippingMethods.OrderBy(x => x.GetShippingTotal(cart)).FirstOrDefault();
        }

        public static IShippingMethod GetMostExpensiveShippingOption(this CartModel cart)
        {
            return cart.PotentiallyAvailableShippingMethods.OrderByDescending(x => x.GetShippingTotal(cart)).FirstOrDefault();
        }

        public static decimal GetCartTaxForPayPal(this CartModel cart)
        {
            return cart.ItemTax;
        }

        public static decimal GetCartTotalForPayPal(this CartModel cart)
        {
            if (cart.ShippingMethod != null)
                return cart.TotalToPay;

            decimal shippingTotal;
            if (cart.ShippingMethod == null)
            {

                var cheapestShippingOption = cart.GetCheapestShippingOption();
                if (cheapestShippingOption == null) return Decimal.Zero;

                shippingTotal = cheapestShippingOption.GetShippingTotal(cart);
            }
            else
            {
                shippingTotal = cart.ShippingTotal;
            }

            return (cart.TotalToPay + shippingTotal);
        }

        public static decimal GetMaxCartTotalForPayPal(this CartModel cart)
        {
            var mostExpensiveShippingOption = cart.GetMostExpensiveShippingOption();
            if (mostExpensiveShippingOption == null) return Decimal.Zero;

            decimal shippingTotal = mostExpensiveShippingOption.GetShippingTotal(cart);
            return (cart.TotalPreShipping + shippingTotal);
        }
    }
}