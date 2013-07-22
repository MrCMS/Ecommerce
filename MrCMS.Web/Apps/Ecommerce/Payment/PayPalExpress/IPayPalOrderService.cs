using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;
using PayPal.PayPalAPIInterfaceService.Model;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public interface IPayPalOrderService
    {
        List<PaymentDetailsType> GetPaymentDetails(CartModel cart);
        string GetBuyerEmail(CartModel cart);
        BasicAmountType GetMaxAmount(CartModel cart);
    }
    public interface IPayPalCartItemService
    {
        PaymentDetailsItemType CreatePaymentItem(CartItem item);
    }

    public class PayPalCartItemService : IPayPalCartItemService
    {
        private readonly PayPalExpressCheckoutSettings _payPalExpressCheckoutSettings;

        public PayPalCartItemService(PayPalExpressCheckoutSettings payPalExpressCheckoutSettings)
        {
            _payPalExpressCheckoutSettings = payPalExpressCheckoutSettings;
        }

        public PaymentDetailsItemType CreatePaymentItem(CartItem item)
        {
            var paymentDetailsItemType = new PaymentDetailsItemType
            {
                Name = item.Name,
                Amount = new BasicAmountType
                {
                    currencyID = _payPalExpressCheckoutSettings.Currency,
                    value = item.Price.ToString()
                },
                ItemCategory = ItemCategoryType.PHYSICAL,
                Quantity = item.Quantity,
                Tax = new BasicAmountType
                {
                    currencyID = _payPalExpressCheckoutSettings.Currency,
                    value = item.Tax.ToString()
                }
            };
            return paymentDetailsItemType;
        }
    }

    public class PayPalOrderService : IPayPalOrderService
    {
        private readonly PayPalExpressCheckoutSettings _payPalExpressCheckoutSettings;
        private readonly IPayPalCartItemService _payPalCartItemService;

        public PayPalOrderService(PayPalExpressCheckoutSettings payPalExpressCheckoutSettings, IPayPalCartItemService payPalCartItemService)
        {
            _payPalExpressCheckoutSettings = payPalExpressCheckoutSettings;
            _payPalCartItemService = payPalCartItemService;
        }

        public List<PaymentDetailsType> GetPaymentDetails(CartModel cart)
        {
            var currencyCode = _payPalExpressCheckoutSettings.Currency;


            return new List<PaymentDetailsType>
                       {
                           new PaymentDetailsType
                               {
                                   OrderTotal =
                                       new BasicAmountType {currencyID = currencyCode, value = cart.Total.ToString()},
                                   ItemTotal =
                                       new BasicAmountType {currencyID = currencyCode, value = cart.Subtotal.ToString()},
                                   ShippingTotal = cart.ShippingTotal.HasValue
                                                       ? new BasicAmountType { currencyID = currencyCode, value = cart.ShippingTotal.ToString() }
                                                       : null,
                                   PaymentDetailsItem = GetPaymentDetailsItems(cart),
                                   PaymentAction = _payPalExpressCheckoutSettings.PaymentAction
                               }
                       };
        }

        private List<PaymentDetailsItemType> GetPaymentDetailsItems(CartModel cart)
        {
            return cart.Items.Select(item => _payPalCartItemService.CreatePaymentItem(item)).ToList();
        }

        public string GetBuyerEmail(CartModel cart)
        {
            return cart.OrderEmail;
        }

        public BasicAmountType GetMaxAmount(CartModel cart)
        {
            return new BasicAmountType(_payPalExpressCheckoutSettings.Currency, cart.Subtotal.ToString("0.00"));
        }
    }
}