using System.ComponentModel;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class ProductAdminSearchQuery
    {
        public ProductAdminSearchQuery()
        {
            Page = 1;
        }

        public int Page { get; set; }

        public PublishStatus PublishStatus { get; set; }

        [DisplayName("Product Name")]
        public string Name { get; set; }

        public string SKU { get; set; }

        public string Brand { get; set; }

        [DisplayName("Category Name")]
        public string CategoryName { get; set; }

        public int? ProductContainerId { get; private set; }

        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
    }
}