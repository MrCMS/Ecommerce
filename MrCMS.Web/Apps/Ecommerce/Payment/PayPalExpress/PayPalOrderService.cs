using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Models;
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
            return new List<PaymentDetailsType>
                       {
                           new PaymentDetailsType
                               {
                                   OrderTotal = cart.Total.GetAmountType(),
                                   ItemTotal = cart.Subtotal.GetAmountType(),
                                   TaxTotal = cart.Tax.GetAmountType(),
                                   ShippingTotal = cart.ShippingTotal.GetAmountType(),
                                   PaymentDetailsItem = GetPaymentDetailsItems(cart),
                                   PaymentAction = _payPalExpressCheckoutSettings.PaymentAction,
                               }
                       };
        }

        private List<PaymentDetailsItemType> GetPaymentDetailsItems(CartModel cart)
        {
            return cart.Items.Select(item => new PaymentDetailsItemType
                                                 {
                                                     Name = item.Name,
                                                     Amount = item.UnitPricePreTax.GetAmountType(),
                                                     ItemCategory = ItemCategoryType.PHYSICAL,
                                                     Quantity = item.Quantity,
                                                     Tax = item.Tax.GetAmountType(),
                                                 }).ToList();
        }

        public string GetBuyerEmail(CartModel cart)
        {
            return cart.OrderEmail;
        }

        public BasicAmountType GetMaxAmount(CartModel cart)
        {
            return cart.Subtotal.GetAmountType();
        }
    }
}