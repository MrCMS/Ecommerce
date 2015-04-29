namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class ETagSearchQuery
    {
        public ETagSearchQuery()
        {
            Page = 1;
        }

        public int Page { get; set; }
        public string Name { get; set; }
    }
}