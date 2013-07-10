namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class ProductVariantPreviousPriceEqOrGrThan0 : ProductVariantEqOrGrThan
    {
        public ProductVariantPreviousPriceEqOrGrThan0()
            : base("Variant Previous Price", o => o.PreviousPrice,0)
        {
        }
    }
}