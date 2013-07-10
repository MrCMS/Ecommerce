namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class ProductVariantMaxSKULengthIs255 : ProductVariantMaxStringLength
    {
        public ProductVariantMaxSKULengthIs255()
            : base("Variant SKU", o => o.SKU, 255)
        {
        }
    }
}