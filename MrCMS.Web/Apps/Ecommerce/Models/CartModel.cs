using System.Collections.Generic;
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

        public decimal TaxSubtotal
        {
            get { return Items.Sum(item => item.Tax); }
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
                                         ? 0
                                         : Discount.GetDiscount(this);
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
            get
            {
                return TotalPreDiscount == 0
                           ? 0
                           : TaxSubtotal * (1 - DiscountAmount / TotalPreDiscount);
            }
        }

        public bool CanCheckout
        {
            get { return Items.Any() && Items.All(item => item.CurrentlyAvailable); }
        }
    }

    public abstract class Discount
    {
        public abstract decimal GetDiscount(CartModel cartModel);
    }
}