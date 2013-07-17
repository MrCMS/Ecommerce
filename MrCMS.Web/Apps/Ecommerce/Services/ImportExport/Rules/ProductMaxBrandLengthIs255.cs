namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class ProductMaxBrandLengthIs255 : ProductMaxStringLength
    {
        public ProductMaxBrandLengthIs255()
            : base("Brand", o => o.Brand, 255)
        {
        }
    }
}