using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress.Helpers;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using PayPal.PayPalAPIInterfaceService.Model;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public class PayPalOrderService : IPayPalOrderService
    {
        private readonly PayPalExpressCheckoutSettings _payPalExpressCheckoutSettings;
        private readonly ICartDiscountApplicationService _cartDiscountApplicationService;

        public PayPalOrderService(PayPalExpressCheckoutSettings payPalExpressCheckoutSettings, ICartDiscountApplicationService cartDiscountApplicationService)
        {
            _payPalExpressCheckoutSettings = payPalExpressCheckoutSettings;
            _cartDiscountApplicationService = cartDiscountApplicationService;
        }

        public List<PaymentDetailsType> GetPaymentDetails(CartModel cart)
        {
            var paymentDetailsType = new PaymentDetailsType
            {
                ItemTotal = GetItemTotal(cart),
                PaymentDetailsItem = GetPaymentDetailsItems(cart),
                PaymentAction = _payPalExpressCheckoutSettings.PaymentAction,
                OrderTotal = cart.GetCartTotalForPayPal().GetAmountType(),
                TaxTotal = cart.GetCartTaxForPayPal().GetAmountType(),
                ShippingTotal = cart.GetShippingTotalForPayPal().GetAmountType(),
                ButtonSource = "Thought_Cart_MrCMS"
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
            {
                paymentDetailsItemTypes.Add(new PaymentDetailsItemType
                {
                    Name = "Order Discount",
                    Amount = (-cart.OrderTotalDiscount).GetAmountType(),
                    ItemCategory = ItemCategoryType.PHYSICAL,
                    Quantity = 1,
                    Tax = 0m.GetAmountType()
                });
            }

            if (cart.ShippingDiscount > 0)
            {
                paymentDetailsItemTypes.Add(new PaymentDetailsItemType
                {
                    Name = "Shipping Discount",
                    Amount = (-cart.ShippingDiscount).GetAmountType(),
                    ItemCategory = ItemCategoryType.PHYSICAL,
                    Quantity = 1,
                    Tax = 0m.GetAmountType()
                });
            }

            paymentDetailsItemTypes.AddRange(from giftCard in cart.AppliedGiftCards
                                             where giftCard.AvailableAmount > 0
                                             select new PaymentDetailsItemType
                                             {
                                                 Name = "Gift Card - " + giftCard.Code,
                                                 Amount = (-giftCard.AvailableAmount).GetAmountType(),
                                                 ItemCategory = ItemCategoryType.PHYSICAL,
                                                 Quantity = 1,
                                                 Tax = 0m.GetAmountType()
                                             });

            if (cart.AppliedRewardPointsAmount > 0)
                paymentDetailsItemTypes.Add(new PaymentDetailsItemType
                {
                    Name = string.Format("Reward Points ({0})", cart.AppliedRewardPoints),
                    Amount = (-cart.AppliedRewardPointsAmount).GetAmountType(),
                    ItemCategory = ItemCategoryType.PHYSICAL,
                    Quantity = 1,
                    Tax = 0m.GetAmountType()
                });

            return paymentDetailsItemTypes;
        }

        public BasicAmountType GetItemTotal(CartModel cart)
        {
            return (cart.Subtotal - cart.OrderTotalDiscount - cart.ShippingDiscount - cart.AppliedRewardPointsAmount - cart.GiftCardAmount).GetAmountType();
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