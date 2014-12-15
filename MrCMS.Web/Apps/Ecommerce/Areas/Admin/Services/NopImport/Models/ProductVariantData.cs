using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models
{
    public class ProductVariantData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public decimal Weight { get; set; }
        public decimal BasePrice { get; set; }
        public decimal PreviousPrice { get; set; }
        public int StockRemaining { get; set; }

        public int TaxRate { get; set; }

        public TrackingPolicy TrackingPolicy { get; set; }

        public string PartNumber { get; set; }

        public string Gtin { get; set; }

        public bool Download { get; set; }

        public bool GiftCard { get; set; }

        public GiftCardType GiftCardType { get; set; }

        public bool RequiresShipping { get; set; }

        public int MaxDownloads { get; set; }

        public int? DownloadDays { get; set; }

        public HashSet<PriceBreakInfo> PriceBreaks { get; set; }

        public string DownloadUrl { get; set; }
    }
}