using MrCMS.Web.Apps.Ecommerce.Models;
using System.Collections.Generic;
namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs
{
    public class ProductVariantImportDataTransferObject 
    {
        public ProductVariantImportDataTransferObject()
        {
            Options = new Dictionary<string, string>();
            PriceBreaks = new Dictionary<int, decimal>();
        }

        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal? PreviousPrice { get; set; }
        public int? TaxRate { get; set; }
        public decimal? Weight { get; set; }
        public int? Stock { get; set; }
        public TrackingPolicy TrackingPolicy { get; set; }
        public string SKU { get; set; }
        public string Barcode { get; set; }
        public Dictionary<string, string> Options { get; set; }
        public Dictionary<int, decimal> PriceBreaks { get; set; }
    }
}