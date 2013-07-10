using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs
{
    /// <summary>
    /// Object that will contain the product data from the spreadsheet
    /// </summary>
    public class ProductImportDataTransferObject
    {
        public List<ProductVariantImportDataTransferObject> ProductVariants { get; set; }

        public string Url { get; set; }

        public string Name { get; set; }

        public string SeoTitle { get; set; }

        public int? TaxRate { get; set; }

        // etc.
    }
}