namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class ProductVariantWeightEqOrGrThan0 : ProductVariantEqOrGrThan
    {
        public ProductVariantWeightEqOrGrThan0()
            : base("Variant Weight", o => o.Weight,0)
        {
        }
    }
}