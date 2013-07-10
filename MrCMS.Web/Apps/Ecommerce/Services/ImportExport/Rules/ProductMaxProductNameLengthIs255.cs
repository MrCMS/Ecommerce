namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class ProductMaxProductNameLengthIs255 : ProductMaxStringLength
    {
        public ProductMaxProductNameLengthIs255()
            : base("Product Name", o => o.Name, 255)
        {
        }
    }
}