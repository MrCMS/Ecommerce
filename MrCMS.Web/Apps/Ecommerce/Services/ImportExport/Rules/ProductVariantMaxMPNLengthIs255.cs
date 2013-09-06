namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class ProductVariantMaxMPNLengthIs255 : ProductVariantMaxStringLength
    {
        public ProductVariantMaxMPNLengthIs255()
            : base("Manufacturer Part Number", o => o.ManufacturerPartNumber, 255)
        {
        }
    }
}