namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class ProductMaxSEOTitleLengthIs250 : ProductMaxStringLength
    {
        public ProductMaxSEOTitleLengthIs250()
            : base("SEO Title", o => o.SEOTitle, 250)
        {
        }
    }
}