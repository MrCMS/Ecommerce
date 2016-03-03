namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class ProductMaxSearchResultAbstractLengthIs500 : ProductMaxStringLength
    {
        public ProductMaxSearchResultAbstractLengthIs500() 
            : base("SearchResultAbstract", o => o.SearchResultAbstract, 500)
        {
        }
    }
}