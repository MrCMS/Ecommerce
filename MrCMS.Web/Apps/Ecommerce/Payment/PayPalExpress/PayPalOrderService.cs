using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress.Helpers;
using MrCMS.Web.Apps.Ecommerce.Settings;
using PayPal.PayPalAPIInterfaceService.Model;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public class PayPalOrderService : IPayPalOrderService
    {
        private readonly PayPalExpressCheckoutSettings _payPalExpressCheckoutSettings;
        private readonly TaxSettings _taxSettings;

        public PayPalOrderService(PayPalExpressCheckoutSettings payPalExpressCheckoutSettings, TaxSettings taxSettings)
        {
            _payPalExpressCheckoutSettings = payPalExpressCheckoutSettings;
            _taxSettings = taxSettings;
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
            var paymentDetailsItemTypes = GetCartItemPaymentDetailsItemTypes(cart);

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

        private List<PaymentDetailsItemType> GetCartItemPaymentDetailsItemTypes(CartModel cart)
        {
            List<PaymentDetailsItemType> paymentDetailsItemTypes = new List<PaymentDetailsItemType>();
            switch (_taxSettings.TaxCalculationMethod)
            {
                case TaxCalculationMethod.Individual:
                    paymentDetailsItemTypes.AddRange(cart.Items.Select(item => new PaymentDetailsItemType
                    {
                        Name = item.Name,
                        Amount = item.UnitPricePreTax.GetAmountType(),
                        ItemCategory = item.RequiresShipping ? ItemCategoryType.PHYSICAL : ItemCategoryType.DIGITAL,
                        Quantity = item.Quantity,
                        Tax = item.UnitTax.GetAmountType(),
                    }));
                    break;
                case TaxCalculationMethod.Row:
                    paymentDetailsItemTypes.AddRange(cart.Items.Select(item => new PaymentDetailsItemType
                    {
                        Name = item.Name + " x" + item.Quantity,
                        Amount = item.PricePreTax.GetAmountType(),
                        ItemCategory = item.RequiresShipping ? ItemCategoryType.PHYSICAL : ItemCategoryType.DIGITAL,
                        Quantity = 1,
                        Tax = item.Tax.GetAmountType(),
                    }));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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