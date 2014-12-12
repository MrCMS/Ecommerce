using System.ComponentModel;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class DiscountSearchQuery
    {
        public DiscountSearchQuery()
        {
            Page = 1;
        }

        public int Page { get; set; }

        [DisplayName("Discount Code")]
        public string DiscountCode { get; set; }

        public string Name { get; set; }

        [DisplayName("Show Expired")]
        public bool ShowExpired { get; set; }
    }
}