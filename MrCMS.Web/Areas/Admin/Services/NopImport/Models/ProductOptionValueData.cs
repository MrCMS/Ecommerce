namespace MrCMS.Web.Areas.Admin.Services.NopImport.Models
{
    public class ProductOptionValueData
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public decimal PriceAdjustment { get; set; }
        public decimal WeightAdjustment { get; set; }


        public int OptionId { get; set; }
        public int VariantId { get; set; }
    }
}