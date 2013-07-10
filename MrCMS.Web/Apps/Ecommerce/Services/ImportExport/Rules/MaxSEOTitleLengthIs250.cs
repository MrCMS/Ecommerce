namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class MaxSEOTitleLengthIs250 : MaxStringLength
    {
        public MaxSEOTitleLengthIs250()
            : base("SEO Title", o => o.SEOTitle, 250)
        {
        }
    }
}