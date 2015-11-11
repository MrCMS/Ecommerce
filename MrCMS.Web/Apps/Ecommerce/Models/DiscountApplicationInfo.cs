using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class DiscountApplicationInfo
    {
        public DiscountApplicationInfo()
        {
            ItemDiscounts = new Dictionary<int, decimal>();
            ItemPercentages = new Dictionary<int, decimal>();
            ItemsFree = new Dictionary<int, int>();
        }

        public decimal OrderTotalDiscount { get; set; }
        public decimal ShippingDiscount { get; set; }
        public Dictionary<int, decimal> ItemDiscounts { get; set; }
        public Dictionary<int, decimal> ItemPercentages { get; set; }
        public Dictionary<int, int> ItemsFree { get; set; }

        public bool IsApplied
        {
            get
            {
                return OrderTotalDiscount > decimal.Zero
                       || ShippingDiscount > decimal.Zero
                       || ItemDiscounts.Values.Any(x => x > decimal.Zero)
                       || ItemPercentages.Values.Any(x => x > decimal.Zero)
                       || ItemsFree.Values.Any(x => x > 0);
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
            foreach (int key in info.ItemPercentages.Keys)
            {
                if (!ItemPercentages.ContainsKey(key))
                    ItemPercentages[key] = info.ItemPercentages[key];
                else
                    ItemPercentages[key] += info.ItemPercentages[key];
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