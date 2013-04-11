using System;
using System.Collections.Generic;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Ecommerce.Entities;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class CartModel
    {
        public CartModel()
        {
            Items = new List<CartItem>();
        }
        public List<CartItem> Items { get; set; }

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
                // get total for orders
                var discountAmount = Discount == null
                                         ? 0
                                         : Discount.GetDiscount(this);
                if (Discount != null && Items.Any())
                {
                    discountAmount += Items.Sum(item => item.GetDiscountAmount(Discount, DiscountCode));
                }
                return discountAmount > TotalPreDiscount
                           ? TotalPreDiscount
                           : discountAmount;
            }
        }

        public decimal Total
        {
            get { return TotalPreDiscount - DiscountAmount; }
        }

        public decimal Tax
        {
            get { return Items.Sum(item => item.Tax); }
        }

        public bool CanCheckout
        {
            get { return Items.Any() && Items.All(item => item.CurrentlyAvailable); }
        }

        public User User { get; set; }

        public Guid UserGuid { get; set; }

        public string DiscountCode { get; set; }
    }
}