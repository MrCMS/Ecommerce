using System;
using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs
{
    /// <summary>
    /// Object that will contain the product data from the spreadsheet
    /// </summary>
    public class ProductImportDataTransferObject
    {
        public ProductImportDataTransferObject()
        {
            ProductVariants = new List<ProductVariantImportDataTransferObject>();
            Categories = new List<string>();
            Specifications = new Dictionary<string, string>();
            Images = new List<string>();
            UrlHistory = new List<string>();
        }

        public List<ProductVariantImportDataTransferObject> ProductVariants { get; set; }

        public string UrlSegment { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SEOTitle { get; set; }
        public string SEODescription { get; set; }
        public string SEOKeywords { get; set; }
        public string Abstract { get; set; }
        public string SearchResultAbstract { get; set; }
        public string Brand { get; set; }
        public List<string> Categories { get; set; }
        public Dictionary<string, string> Specifications { get; set; }
        public List<string> Images { get; set; }
        public List<string> UrlHistory { get; set; }
        public DateTime? PublishDate { get; set; }

        public IEnumerable<string> Options
        {
            get { return ProductVariants.SelectMany(o => o.Options.Keys).Distinct(StringComparer.OrdinalIgnoreCase); }
        }
    }
}