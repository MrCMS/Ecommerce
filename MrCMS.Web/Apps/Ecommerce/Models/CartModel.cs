using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Helpers.Cart;
using MrCMS.Web.Apps.Ecommerce.Payment;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class CartModel
    {

        public CartModel()
        {
            Items = new List<CartItemData>();
            AvailablePaymentMethods = new List<BasePaymentMethod>();
            AppliedGiftCards = new List<GiftCard>();
            PotentiallyAvailableShippingMethods = new HashSet<IShippingMethod>();
        }

        // user & items
        public Guid CartGuid { get; set; }
        public List<CartItemData> Items { get; set; }
        public User User { get; set; }
        public Guid UserGuid { get; set; }
        public bool TermsAndConditionsRequired { get; set; }
        public bool TermsAndConditionsAccepted { get; set; }

        /// <summary>
        ///     Discount from order total
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:£0.00}")]
        public virtual decimal OrderTotalDiscount { get; set; }

        /// <summary>
        ///     Shipping Total
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:£0.00}")]
        public virtual decimal ShippingDiscount { get; set; }

        [Required]
        public string OrderEmail { get; set; }

        public string GiftMessage { get; set; }

        [DisplayName("Billing Address same as Shipping Address?")]
        public bool BillingAddressSameAsShippingAddress { get; set; }

        // discounts
        public List<DiscountInfo> Discounts { get; set; }

        [Required]
        [DisplayName("Discount Codes")]
        public List<string> DiscountCodes { get; set; }

        // shipping
        public Address ShippingAddress { get; set; }
        public IShippingMethod ShippingMethod { get; set; }
        public HashSet<IShippingMethod> PotentiallyAvailableShippingMethods { get; set; }
        public DateTime? RequestedShippingDate { get; set; }

        // billing
        public Address BillingAddress { get; set; }
        public BasePaymentMethod PaymentMethod { get; set; }
        public IEnumerable<BasePaymentMethod> AvailablePaymentMethods { get; set; }
        public string PayPalExpressToken { get; set; }
        public string PayPalExpressPayerId { get; set; }
        public bool AnyStandardPaymentMethodsAvailable { get; set; }
        public bool PayPalExpressAvailable { get; set; }

        //gift cards
        public List<GiftCard> AppliedGiftCards { get; set; }

        /// <summary>
        ///     Item total less tax (also factors in discounts)
        /// </summary>
        public decimal Subtotal
        {
            get { return Items.Sum(item => item.PricePreTax); }
        }

        /// <summary>
        ///     Item total including tax (also factors in discounts)
        /// </summary>
        public virtual decimal TotalPreDiscount
        {
            get { return Items.Sum(item => item.Price); }
        }

        /// <summary>
        ///     Item total less order total discount
        /// </summary>
        public virtual decimal TotalPreShipping
        {
            get { return TotalPreDiscount - OrderTotalDiscount; }
        }

        /// <summary>
        ///     Item total less order total discount plus shipping total
        /// </summary>
        public virtual decimal Total
        {
            get { return TotalPreShipping + ShippingTotal; }
        }

        public virtual decimal GiftCardAmount
        {
            get { return AvailableGiftCardAmount > Total ? Total : AvailableGiftCardAmount; }
        }

        /// <summary>
        ///     Total available amount on applied gift cards
        /// </summary>
        public virtual decimal AvailableGiftCardAmount
        {
            get { return AppliedGiftCards.Sum(x => x.AvailableAmount); }
        }

        /// <summary>
        ///     The order total less the applied gift card amount
        /// </summary>
        public virtual decimal TotalToPay
        {
            get { return TotalLessGiftCardAmount - AppliedRewardPointsAmount; }
        }


        public virtual decimal TotalLessGiftCardAmount
        {
            get { return Total - GiftCardAmount; }
        }

        public virtual int AppliedRewardPoints
        {
            get
            {
                if (!UseRewardPoints || AppliedRewardPointsAmount == decimal.Zero)
                    return 0;
                return (int)Math.Ceiling(AppliedRewardPointsAmount / RewardPointsExchangeRate);
            }
        }

        public virtual decimal AppliedRewardPointsAmount
        {
            get
            {
                if (!UseRewardPoints)
                    return decimal.Zero;
                return TotalLessGiftCardAmount >= AvailablePointsValue
                    ? AvailablePointsValue
                    : TotalLessGiftCardAmount;
            }
        }

        /// <summary>
        ///     Item tax plus shipping tax
        /// </summary>
        public decimal Tax
        {
            get { return ItemTax + ShippingTax; }
        }

        /// <summary>
        ///     Item tax
        /// </summary>
        public virtual decimal ItemTax
        {
            get { return Items.Sum(item => item.Tax); }
        }

        public IDictionary<decimal, decimal> TaxRates
        {
            get
            {
                return Items.GroupBy(item => item.TaxRatePercentage)
                    .ToDictionary(items => items.Key,
                        items => items.Sum(item => item.Price));
            }
        }

        /// <summary>
        ///     Total discount amount (including item discounts)
        /// </summary>
        public decimal DiscountAmount
        {
            get
            {
                decimal discountAmount = OrderTotalDiscount + ShippingDiscount + ItemDiscount;

                return discountAmount > TotalPreDiscount
                    ? TotalPreDiscount
                    : discountAmount;
            }
        }

        /// <summary>
        ///     Discount from Items
        /// </summary>
        public virtual decimal ItemDiscount
        {
            get
            {
                return Items.Any()
                    ? Items.Sum(item => item.DiscountAmount)
                    : decimal.Zero;
            }
        }

        /// <summary>
        ///     Shipping Total
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:£0.00}")]
        public virtual decimal ShippingTotalPreDiscount
        {
            get
            {
                return ShippingMethod == null
                    ? decimal.Zero
                    : ShippingMethod.GetShippingTotal(this);
            }
        }

        /// <summary>
        ///     Shipping Total
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:£0.00}")]
        public virtual decimal ShippingTotal
        {
            get
            {
                return ShippingDiscount >= ShippingTotalPreDiscount
                    ? decimal.Zero
                    : ShippingTotalPreDiscount - ShippingDiscount;
            }
        }

        /// <summary>
        ///     Shipping Tax
        /// </summary>
        public virtual decimal ShippingTax
        {
            get
            {
                return ShippingMethod == null
                    ? decimal.Zero
                    : ShippingMethod.GetShippingTax(this);
            }
        }

        /// <summary>
        ///     Shipping Pre tax
        /// </summary>
        public decimal ShippingPreTax
        {
            get { return ShippingTotal - ShippingTax; }
        }

        /// <summary>
        ///     Shipping tax percentage
        /// </summary>
        public decimal ShippingTaxPercentage
        {
            get
            {
                return ShippingMethod == null
                    ? decimal.Zero
                    : ShippingMethod.TaxRatePercentage;
            }
        }


        public int ItemQuantity
        {
            get { return Items.Sum(item => item.Quantity); }
        }


        public bool Empty
        {
            get { return !Items.Any(); }
        }


        public CartShippingStatus ShippingStatus
        {
            get
            {
                if (!RequiresShipping)
                {
                    return CartShippingStatus.ShippingNotRequired;
                }
                if (ShippingMethod != null)
                {
                    return CartShippingStatus.ShippingSet;
                }
                if (!PotentiallyAvailableShippingMethods.Any())
                {
                    return CartShippingStatus.CannotShip;
                }
                return CartShippingStatus.ShippingNotSet;
            }
        }

        public virtual decimal Weight
        {
            get { return Items.Sum(item => item.Weight); }
        }
               
        public string PaymentMethodSystemName
        {
            get
            {
                return PaymentMethod == null
                    ? string.Empty
                    : PaymentMethod.SystemName;
            }
        }

        public string PaymentMethodAction
        {
            get
            {
                return PaymentMethod == null
                    ? string.Empty
                    : PaymentMethod.ActionName;
            }
        }

        public string PaymentMethodController
        {
            get
            {
                return PaymentMethod == null
                    ? string.Empty
                    : PaymentMethod.ControllerName;
            }
        }

        public bool IsPayPalTransaction
        {
            get
            {
                return !string.IsNullOrWhiteSpace(PayPalExpressToken) && !string.IsNullOrWhiteSpace(PayPalExpressPayerId);
            }
        }


        public virtual bool RequiresShipping
        {
            get { return Items.Any(item => item.RequiresShipping); }
        }


        public bool CanCheckout
        {
            get { return !CannotCheckoutReasons.Any(); }
        }

        public bool CanEnterPaymentFlow
        {
            get { return CanCheckout && AnyStandardPaymentMethodsAvailable; }
        }

        public bool CanUsePayPalExpress
        {
            get { return CanCheckout && PayPalExpressAvailable; }
        }

        public bool CanPlaceOrder
        {
            get { return !CannotPlaceOrderReasons.Any(); }
        }

        public IEnumerable<string> CannotCheckoutReasons
        {
            get
            {
                if (!Items.Any())
                    yield return "You have nothing in your cart";
                foreach (CartItemData item in Items.Where(item => !item.CanBuy))
                {
                    yield return item.Error;
                }
                if (ShippingStatus == CartShippingStatus.CannotShip)
                    yield return "You are currently unable to ship the items in your cart";
            }
        }

        public IEnumerable<string> CannotPlaceOrderReasons
        {
            get
            {
                foreach (string cannotCheckoutReason in CannotCheckoutReasons)
                    yield return cannotCheckoutReason;
                if (RequiresShipping && ShippingAddress == null)
                    yield return "Shipping address is not set";
                if (RequiresShipping && ShippingMethod == null)
                    yield return "Shipping method is not set";
                if (BillingAddress == null)
                    yield return "Billing address is not set";
                if (MrCMSApplication.Get<ISession>().QueryOver<Order>().Where(order => order.Guid == CartGuid).Any())
                    yield return "Order has already been placed";
                if (!this.TermsAcceptanceIsValid())
                    yield return "You have not accepted the terms and conditions";

            }
        }

        [DisplayName("Use reward points?")]
        public bool UseRewardPoints { get; set; }

        [DisplayName("Available points")]
        public int AvailablePoints { get; set; }

        [DisplayName("Available points value")]
        public decimal AvailablePointsValue { get; set; }

        public decimal RewardPointsExchangeRate { get; set; }



        public void SetDiscountApplication(DiscountApplicationInfo discountApplicationInfo)
        {
            Items.ForEach(item => item.ResetDiscountInfo());

            OrderTotalDiscount = discountApplicationInfo.OrderTotalDiscount;
            ShippingDiscount = discountApplicationInfo.ShippingDiscount;
            foreach (var key in discountApplicationInfo.ItemDiscounts.Keys)
            {
                var cartItem = Items.FirstOrDefault(x => x.Id == key);
                if (cartItem != null)
                    cartItem.SetDiscountAmount(discountApplicationInfo.ItemDiscounts[key]);
            }
            foreach (var key in discountApplicationInfo.ItemPercentages.Keys)
            {
                var cartItem = Items.FirstOrDefault(x => x.Id == key);
                if (cartItem != null)
                    cartItem.SetDiscountPercentage(discountApplicationInfo.ItemPercentages[key]);
            }
            foreach (var key in discountApplicationInfo.ItemsFree.Keys)
            {
                var item = Items.FirstOrDefault(x => x.Id == key);
                if (item != null)
                    item.SetFreeItems(discountApplicationInfo.ItemsFree[key]);
            }
        }
    }
}