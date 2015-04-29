using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Ecommerce.Pages
{
    public class BrandListing : Webpage, IUniquePage
    {
        public BrandListing()
        {
            PageSize = 50;
        }

        public virtual int PageSize { get; set; }
    }
}