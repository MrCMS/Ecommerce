using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress.Helpers;
using PayPal.PayPalAPIInterfaceService.Model;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
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
                Amount = item.UnitPricePreTax.GetAmountType(),
                ItemCategory = item.RequiresShipping ? ItemCategoryType.PHYSICAL : ItemCategoryType.DIGITAL,
                Quantity = item.Quantity,
                Tax = item.UnitTax.GetAmountType(),
            }).ToList();
            if (cart.OrderTotalDiscount > 0)
                paymentDetailsItemTypes.Add(new PaymentDetailsItemType
                                                {
                                                    Name = "Discount - " + cart.DiscountCodes,
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
            return cart.GetMaxCartTotalForPayPal().GetAmountType();
        }
    }
}