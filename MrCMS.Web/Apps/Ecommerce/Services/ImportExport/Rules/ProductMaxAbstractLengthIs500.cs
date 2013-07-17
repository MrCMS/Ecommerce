namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class ProductMaxAbstractLengthIs500 : ProductMaxStringLength
    {
        public ProductMaxAbstractLengthIs500()
            : base("Abstract", o => o.Abstract, 500)
        {
        }
    }
}