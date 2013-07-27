using System;
using System.Collections.Generic;
using MrCMS.Entities.People;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MrCMS.Web.Apps.Ecommerce.Payment;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class CartModel
    {
        public CartModel()
        {
            Items = new List<CartItem>();
        }
        public List<CartItem> Items { get; set; }
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

        public IDictionary<decimal, decimal> TaxRates
        {
            get
            {
                return Items.GroupBy(item => item.TaxRatePercentage)
                            .ToDictionary(items => items.Key,
                                          items => items.Sum(item => item.Price));
            }
        }

        public Discount Discount { get; set; }

        public decimal DiscountAmount
        {
            get
            {
                var discountAmount = Discount == null
                                         ? decimal.Zero
                                         : Discount.GetDiscount(this);

                if (Discount != null && Items.Any())
                    discountAmount += Items.Sum(item => item.GetDiscountAmount(Discount, DiscountCode));

                return discountAmount > TotalPreDiscount
                           ? TotalPreDiscount
                           : discountAmount;
            }
        }

        public virtual decimal Total
        {
            get { return TotalPreShipping + ShippingTotal.GetValueOrDefault(); }
        }

        public virtual decimal TotalPreShipping
        {
            get { return TotalPreDiscount - DiscountAmount; }
        }

        public decimal Tax
        {
            get
            {
                return Items.Sum(item => item.Tax) + ShippingTax.GetValueOrDefault();
            }
        }

        public bool CanCheckout
        {
            get { return Items.Any() && Items.All(item => item.CurrentlyAvailable); }
        }

        public User User { get; set; }

        public Guid UserGuid { get; set; }

        [Required]
        [DisplayName("Discount Code")]
        public string DiscountCode { get; set; }

        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        public string OrderEmail { get; set; }

        public Address ShippingAddress { get; set; }
        public Address BillingAddress { get; set; }

        public ShippingMethod ShippingMethod { get; set; }

        [DisplayFormat(DataFormatString = "{0:£0.00}")]
        public decimal? ShippingTotal { get { return ShippingMethod == null ? null : ShippingMethod.GetPrice(this); } }
        public decimal? ShippingTax { get { return ShippingMethod == null ? null : ShippingMethod.GetTax(this); } }
        public decimal? ShippingTaxPercentage
        {
            get { return ShippingTotal.GetValueOrDefault() == 0 ? (decimal?)null : ShippingTax.GetValueOrDefault() / ShippingTotal.GetValueOrDefault(); }
        }

        public virtual decimal Weight
        {
            get { return Items.Any() ? Items.Sum(item => item.Weight) : decimal.Zero; }
        }

        public bool AnyStandardPaymentMethodsAvailable { get; set; }
        public bool CanEnterPaymentFlow { get { return Items.Any() && AnyStandardPaymentMethodsAvailable; } }
        public bool PayPalExpressAvailable { get; set; }
        public bool CanUsePayPalExpress { get { return Items.Any() && PayPalExpressAvailable; } }

        public IEnumerable<IPaymentMethod> AvailablePaymentMethods { get; set; }

        public Country Country { get; set; }

        public string PaymentMethod { get; set; }

        [DisplayName("Billing Address same as Shipping Address?")]
        public bool BillingAddressSameAsShippingAddress { get; set; }

        public bool NeedToSetBillingAddress { get { return !BillingAddressSameAsShippingAddress && BillingAddress == null; } }

        public bool HasDiscount
        {
            get { return Discount != null && DiscountAmount != decimal.Zero; }
        }

        public decimal ItemQuantity
        {
            get { return Items.Sum(item => item.Quantity); }
        }

        public string PayPalExpressToken { get; set; }
        public string PayPalExpressPayerId { get; set; }
    }
}