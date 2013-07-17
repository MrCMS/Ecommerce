namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class ProductVariantPriceEqOrGrThan0 : ProductVariantEqOrGrThan
    {
        public ProductVariantPriceEqOrGrThan0()
            : base("Variant Price", o => o.Price,0)
        {
        }
    }
}