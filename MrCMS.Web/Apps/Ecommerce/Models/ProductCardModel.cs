using MrCMS.Web.Apps.Ecommerce.Entities.ETags;
using MrCMS.Web.Apps.Ecommerce.Models.StockAvailability;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class ProductCardModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Abstract { get; set; }
        public decimal? Price { get; set; }
        public decimal? Tax { get; set; }
        public decimal? PricePreTax { get; set; }
        public decimal? PreviousPrice { get; set; }
        public string Image { get; set; }
        public int? VariantId { get; set; }
        public string PreviousPriceText { get; set; }
        public bool IsMultiVariant { get; set; }
        public CanBuyStatus CanBuyStatus { get; set; }
        public string StockMessage { get; set; }
        public bool ProductReviewsEnabled { get; set; }
        public decimal Rating { get; set; }
        public int NumberOfReviews { get; set; }

        public ETag ETag { get; set; }
    }
}