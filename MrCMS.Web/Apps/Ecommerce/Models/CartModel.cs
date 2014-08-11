using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models.Shipping;
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
            Items = new List<CartItem>();
            AvailablePaymentMethods = new List<IPaymentMethod>();
        }

        // user & items
        public Guid CartGuid { get; set; }
        public List<CartItem> Items { get; set; }
        public User User { get; set; }
        public Guid UserGuid { get; set; }

        [Required]
        public string OrderEmail { get; set; }

        [DisplayName("Billing Address same as Shipping Address?")]
        public bool BillingAddressSameAsShippingAddress { get; set; }

        // discounts
        public Discount Discount { get; set; }

        [Required]
        [DisplayName("Discount Code")]
        public string DiscountCode { get; set; }

        // shipping
        public Address ShippingAddress { get; set; }
        public IShippingMethod ShippingMethod { get; set; }
        public HashSet<IShippingMethod> PotentiallyAvailableShippingMethods { get; set; }

        // billing
        public Address BillingAddress { get; set; }
        public IPaymentMethod PaymentMethod { get; set; }
        public IEnumerable<IPaymentMethod> AvailablePaymentMethods { get; set; }
        public string PayPalExpressToken { get; set; }
        public string PayPalExpressPayerId { get; set; }
        public bool AnyStandardPaymentMethodsAvailable { get; set; }
        public bool PayPalExpressAvailable { get; set; }



        public IEnumerable<CartItem> ShippableItems
        {
            get { return Items.Where(item => item.RequiresShipping); }
        }

        public bool Empty
        {
            get { return !Items.Any(); }
        }

        public decimal Subtotal
        {
            get { return Items.Sum(item => item.PricePreTax); }
        }

        public decimal TotalPreDiscount
        {
            get { return Items.Sum(item => item.Price); }
        }

        public virtual decimal ShippableTotalPreDiscount
        {
            get { return ShippableItems.Sum(item => item.Price); }
        }

        public virtual decimal ShippableCalculationTotal
        {
            get { return ShippableTotalPreDiscount - OrderTotalDiscount; }
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

        public decimal DiscountAmount
        {
            get
            {
                decimal discountAmount = OrderTotalDiscount;

                discountAmount += ItemDiscount;

                return discountAmount > TotalPreDiscount
                    ? TotalPreDiscount
                    : discountAmount;
            }
        }

        public decimal OrderTotalDiscount
        {
            get
            {
                return Discount == null
                    ? decimal.Zero
                    : Discount.GetDiscount(this);
            }
        }

        public decimal ItemDiscount
        {
            get
            {
                return Discount != null && Items.Any()
                    ? Items.Sum(item => item.DiscountAmount)
                    : decimal.Zero;
            }
        }

        public virtual decimal Total
        {
            get { return TotalPreShipping + ShippingTotal.GetValueOrDefault(); }
        }

        public virtual decimal TotalPreShipping
        {
            get { return TotalPreDiscount - OrderTotalDiscount; }
        }

        public decimal Tax
        {
            get { return ItemTax + ShippingTax.GetValueOrDefault(); }
        }

        public decimal ItemTax
        {
            get { return Items.Sum(item => item.Tax); }
        }

        public CartShippingStatus ShippingStatus
        {
            get
            {
                if (!RequiresShipping)
                {
                    return CartShippingStatus.ShippingNotRequired;
                }
                if (!PotentiallyAvailableShippingMethods.Any())
                {
                    return CartShippingStatus.CannotShip;
                }
                if (ShippingMethod == null)
                {
                    return CartShippingStatus.ShippingNotSet;
                }
                return CartShippingStatus.ShippingSet;
            }
        }

        [DisplayFormat(DataFormatString = "{0:£0.00}")]
        public decimal? ShippingTotal
        {
            get
            {
                if (ShippingMethod == null) return null;
                ShippingAmount shippingAmount = ShippingMethod.GetShippingTotal(this);
                return shippingAmount == ShippingAmount.NoneAvailable
                    ? (decimal?) null
                    : shippingAmount.Value;
            }
        }

        public decimal? ShippingTax
        {
            get
            {
                if (ShippingMethod == null) return null;
                ShippingAmount shippingAmount = ShippingMethod.GetShippingTax(this);
                return shippingAmount == ShippingAmount.NoneAvailable
                    ? (decimal?) null
                    : shippingAmount.Value;
            }
        }

        public decimal? ShippingPreTax
        {
            get { return ShippingTotal - ShippingTax; }
        }

        public decimal? ShippingTaxPercentage
        {
            get { return ShippingMethod == null ? (decimal?) null : ShippingMethod.TaxRatePercentage; }
        }

        public virtual decimal Weight
        {
            get { return Items.Sum(item => item.Weight); }
        }

        public bool PaymentMethodSet
        {
            get { return PaymentMethod != null; }
        }

        public string PaymentMethodSystemName
        {
            get { return PaymentMethod == null ? string.Empty : PaymentMethod.SystemName; }
        }

        public string PaymentMethodAction
        {
            get { return PaymentMethod == null ? string.Empty : PaymentMethod.ActionName; }
        }

        public string PaymentMethodController
        {
            get { return PaymentMethod == null ? string.Empty : PaymentMethod.ControllerName; }
        }

        public bool NeedToSetBillingAddress
        {
            get { return !BillingAddressSameAsShippingAddress && BillingAddress == null; }
        }

        public bool HasDiscount
        {
            get { return Discount != null && OrderTotalDiscount != decimal.Zero; }
        }

        public decimal ItemQuantity
        {
            get { return Items.Sum(item => item.Quantity); }
        }


        public bool IsPayPalTransaction
        {
            get
            {
                return !string.IsNullOrWhiteSpace(PayPalExpressToken) && !string.IsNullOrWhiteSpace(PayPalExpressPayerId);
            }
        }

        public bool RequiresShipping
        {
            get { return ShippableItems.Any(); }
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
                foreach (CartItem item in Items)
                {
                    if (!item.CanBuy(this))
                        yield return item.Error(this);
                }
                if(ShippingStatus == CartShippingStatus.CannotShip)
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
            }
        }

    }

    public enum CartShippingStatus
    {
        ShippingNotRequired,
        CannotShip,
        ShippingNotSet,
        ShippingSet
    }
}