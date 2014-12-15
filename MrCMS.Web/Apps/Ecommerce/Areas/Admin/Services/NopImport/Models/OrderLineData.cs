namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models
{
    public class OrderLineData
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPriceInclTax { get; set; }

        public decimal UnitPriceExclTax { get; set; }

        public decimal PriceInclTax { get; set; }

        public decimal PriceExclTax { get; set; }

        public decimal DiscountAmountInclTax { get; set; }

        public decimal DiscountAmountExclTax { get; set; }

        public int DownloadCount { get; set; }

        public decimal? ItemWeight { get; set; }

        public bool RequiresShipping { get; set; }

        public string SKU { get; set; }
        public string ProductName { get; set; }
    }
}