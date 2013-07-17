namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class ProductVariantMaxNameLengthIs255 : ProductVariantMaxStringLength
    {
        public ProductVariantMaxNameLengthIs255()
            : base("Variant Name", o => o.Name, 255)
        {
        }
    }
}