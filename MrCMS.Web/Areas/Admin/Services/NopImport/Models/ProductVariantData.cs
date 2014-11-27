using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Areas.Admin.Services.NopImport.Nop280;

namespace MrCMS.Web.Areas.Admin.Services.NopImport.Models
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

        public List<PriceBreakInfo> PriceBreaks { get; set; }

        public string DownloadUrl { get; set; }
    }
}