using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class DiscountApplicationResult
    {
        public DiscountApplicationInfo Info { get; set; }
        public List<Discount> AppliedDiscounts { get; set; }
    }
}