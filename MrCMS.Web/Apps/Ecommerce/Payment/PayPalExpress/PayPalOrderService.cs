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
            var applications = (from discountInfo in cart.Discounts
                                let info = _cartDiscountApplicationService.ApplyDiscount(discountInfo, cart)
                                select new { info, discountInfo }).ToHashSet();
            paymentDetailsItemTypes.AddRange(from application in applications

                                             where application.info.OrderTotalDiscount > 0
                                             select new PaymentDetailsItemType
                                             {
                                                 Name = "Order Total Discount - " + application.discountInfo.Discount.Name,
                                                 Amount = (-application.info.OrderTotalDiscount).GetAmountType(),
                                                 ItemCategory = ItemCategoryType.PHYSICAL,
                                                 Quantity = 1,
                                                 Tax = 0m.GetAmountType()
                                             });

            paymentDetailsItemTypes.AddRange(from application in applications
                                             where application.info.ShippingDiscount > 0
                                             select new PaymentDetailsItemType
                                             {
                                                 Name = "Shipping Discount - " + application.discountInfo.Discount.Name,
                                                 Amount = (-application.info.ShippingDiscount).GetAmountType(),
                                                 ItemCategory = ItemCategoryType.PHYSICAL,
                                                 Quantity = 1,
                                                 Tax = 0m.GetAmountType()
                                             });

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