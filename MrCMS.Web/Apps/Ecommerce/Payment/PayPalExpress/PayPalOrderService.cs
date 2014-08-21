using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using PayPal.PayPalAPIInterfaceService.Model;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public static class PayPalCartModelExtensionMethods
    {
        public static decimal GetShippingTotalForPayPal(this CartModel cart)
        {
            if (cart.ShippingMethod != null)
                return cart.ShippingTotal;

            var cheapestShippingOption = cart.GetCheapestShippingOption();
            if (cheapestShippingOption == null) return Decimal.Zero;

            return cheapestShippingOption.GetShippingTotal(cart);

        }

        public static IShippingMethod GetCheapestShippingOption(this CartModel cart)
        {
            return cart.PotentiallyAvailableShippingMethods.OrderBy(x => x.GetShippingTotal(cart)).FirstOrDefault();
        }

        public static decimal GetCartTaxForPayPal(this CartModel cart)
        {
            return cart.ItemTax;
        }

        public static decimal GetCartTotalForPayPal(this CartModel cart)
        {
            if (cart.ShippingMethod != null)
                return cart.Total;

            var cheapestShippingOption = cart.GetCheapestShippingOption();
            if (cheapestShippingOption == null) return Decimal.Zero;

            decimal shippingTotal = cheapestShippingOption.GetShippingTotal(cart);
            return (cart.Total + shippingTotal);
        }
    }
    public class PayPalOrderService : IPayPalOrderService
    {
        private readonly PayPalExpressCheckoutSettings _payPalExpressCheckoutSettings;

        public PayPalOrderService(PayPalExpressCheckoutSettings payPalExpressCheckoutSettings)
        {
            _payPalExpressCheckoutSettings = payPalExpressCheckoutSettings;
        }

        public List<PaymentDetailsType> GetPaymentDetails(CartModel cart)
        {
            var paymentDetailsType = new PaymentDetailsType
            {
                ItemTotal = (cart.Subtotal - cart.OrderTotalDiscount).GetAmountType(),
                PaymentDetailsItem = GetPaymentDetailsItems(cart),
                PaymentAction = _payPalExpressCheckoutSettings.PaymentAction,
                OrderTotal = cart.GetCartTotalForPayPal().GetAmountType(),
                TaxTotal = cart.GetCartTaxForPayPal().GetAmountType(),
                ShippingTotal = cart.GetShippingTotalForPayPal().GetAmountType()
            };


            return new List<PaymentDetailsType>
                       {
                           paymentDetailsType
                       };
        }

        public List<PaymentDetailsItemType> GetPaymentDetailsItems(CartModel cart)
        {
            var paymentDetailsItemTypes = cart.Items.Select(item => new PaymentDetailsItemType
                                                                        {
                                                                            Name = item.Name,
                                                                            Amount =
                                                                                item.UnitPricePreTax.GetAmountType(),
                                                                            ItemCategory = ItemCategoryType.PHYSICAL,
                                                                            Quantity = item.Quantity,
                                                                            Tax = item.UnitTax.GetAmountType(),
                                                                        }).ToList();
            if (cart.OrderTotalDiscount > 0)
                paymentDetailsItemTypes.Add(new PaymentDetailsItemType
                                                {
                                                    Name = "Discount - " + cart.DiscountCode,
                                                    Amount = (-cart.OrderTotalDiscount).GetAmountType(),
                                                    ItemCategory = ItemCategoryType.PHYSICAL,
                                                    Quantity = 1,
                                                    Tax = 0m.GetAmountType()
                                                });
            return paymentDetailsItemTypes;
        }

        public string GetBuyerEmail(CartModel cart)
        {
            return cart.OrderEmail;
        }

        public BasicAmountType GetMaxAmount(CartModel cart)
        {
            return cart.GetCartTotalForPayPal().GetAmountType();
        }
    }
}