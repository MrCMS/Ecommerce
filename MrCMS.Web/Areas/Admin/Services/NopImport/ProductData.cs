using System.Collections.Generic;

namespace MrCMS.Web.Areas.Admin.Services.NopImport
{
    public class ProductData
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Abstract { get; set; }
        public string Description { get; set; }
        public List<ProductVariantData> ProductVariants { get; set; }
    }
}