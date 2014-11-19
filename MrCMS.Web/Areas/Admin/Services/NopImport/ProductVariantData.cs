namespace MrCMS.Web.Areas.Admin.Services.NopImport
{
    public class ProductVariantData
    {
        public string Name { get; set; }
        public string SKU { get; set; }
        public decimal Weight { get; set; }
        public decimal BasePrice { get; set; }
        public decimal PreviousPrice { get; set; }
        public decimal TaxRatePercentage { get; set; }
        public int StockRemaining { get; set; }
    }
}