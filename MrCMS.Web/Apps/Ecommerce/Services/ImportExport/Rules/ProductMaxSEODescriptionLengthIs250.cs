namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class ProductMaxSEODescriptionLengthIs250 : ProductMaxStringLength
    {
        public ProductMaxSEODescriptionLengthIs250()
            : base("SEO Description", o => o.SEODescription, 250)
        {
        }
    }
}