namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class ProductVariantMaxBarcodeLengthIs14 : ProductVariantMaxStringLength
    {
        public ProductVariantMaxBarcodeLengthIs14()
            : base("Variant SKU", o => o.Barcode, 255)
        {
        }
    }
}