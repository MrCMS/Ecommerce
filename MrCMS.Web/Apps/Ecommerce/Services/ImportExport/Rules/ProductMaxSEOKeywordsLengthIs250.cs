namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class ProductMaxSEOKeywordsLengthIs250 : ProductMaxStringLength
    {
        public ProductMaxSEOKeywordsLengthIs250()
            : base("SEO Keywords", o => o.SEOKeywords, 250)
        {
        }
    }
}