using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class DiscountApplicationInfo
    {
        public void Add(DiscountApplicationInfo info)
        {
            OrderTotalDiscount += info.OrderTotalDiscount;
            ShippingDiscount += info.ShippingDiscount;
            foreach (var key in info.ItemDiscounts.Keys)
            {
                if (!ItemDiscounts.ContainsKey(key))
                    ItemDiscounts[key] = info.ItemDiscounts[key];
                else
                    ItemDiscounts[key] += info.ItemDiscounts[key];
            }
        }

        public DiscountApplicationInfo()
        {
            ItemDiscounts = new Dictionary<int, decimal>();
        }
        public decimal OrderTotalDiscount { get; set; }
        public decimal ShippingDiscount { get; set; }
        public Dictionary<int, decimal> ItemDiscounts { get; set; }

        public bool IsApplied
        {
            get
            {
                return OrderTotalDiscount > decimal.Zero
                       || ShippingDiscount > decimal.Zero
                       || ItemDiscounts.Keys.Any(x => ItemDiscounts[x] > decimal.Zero);
            }
        }
    }
}