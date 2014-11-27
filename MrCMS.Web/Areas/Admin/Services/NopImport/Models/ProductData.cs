using System.Collections.Generic;

namespace MrCMS.Web.Areas.Admin.Services.NopImport.Models
{
    public class ProductData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abstract { get; set; }
        public string Description { get; set; }
        public List<ProductVariantData> ProductVariants { get; set; }
        public bool Published { get; set; }

        public int? BrandId { get; set; }
        public List<int> Tags { get; set; }
        public List<int> Categories { get; set; }
    }
}