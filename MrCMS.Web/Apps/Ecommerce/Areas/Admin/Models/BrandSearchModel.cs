namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class BrandSearchModel
    {
        public BrandSearchModel()
        {
            Page = 1;
        }

        public int Page { get; set; }
        public string Query { get; set; }
    }
}