using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class DiscountApplicationInfo
    {
        public DiscountApplicationInfo()
        {
            ItemDiscounts = new Dictionary<int, decimal>();
            ItemsFree = new Dictionary<int, int>();
        }

        public decimal OrderTotalDiscount { get; set; }
        public decimal ShippingDiscount { get; set; }
        public Dictionary<int, decimal> ItemDiscounts { get; set; }
        public Dictionary<int, int> ItemsFree { get; set; }

        public bool IsApplied
        {
            get
            {
                return OrderTotalDiscount > decimal.Zero
                       || ShippingDiscount > decimal.Zero
                       || ItemDiscounts.Keys.Any(x => ItemDiscounts[x] > decimal.Zero)
                       || ItemsFree.Keys.Any(x => ItemsFree[x] > 0);
            }
        }

        public void Add(DiscountApplicationInfo info)
        {
            OrderTotalDiscount += info.OrderTotalDiscount;
            ShippingDiscount += info.ShippingDiscount;
            foreach (int key in info.ItemDiscounts.Keys)
            {
                if (!ItemDiscounts.ContainsKey(key))
                    ItemDiscounts[key] = info.ItemDiscounts[key];
                else
                    ItemDiscounts[key] += info.ItemDiscounts[key];
            }
            foreach (int key in info.ItemsFree.Keys)
            {
                if (!ItemsFree.ContainsKey(key))
                    ItemsFree[key] = info.ItemsFree[key];
                else
                    ItemsFree[key] += info.ItemsFree[key];
            }
        }
    }
}