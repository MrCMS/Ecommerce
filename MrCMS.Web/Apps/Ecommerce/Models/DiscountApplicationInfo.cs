using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class DiscountApplicationInfo
    {
        public void Add(DiscountApplicationInfo info)
        {
            OrderDiscount += info.OrderDiscount;
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
        public decimal OrderDiscount { get; set; }
        public decimal ShippingDiscount { get; set; }
        public Dictionary<int, decimal> ItemDiscounts { get; set; }
    }
}