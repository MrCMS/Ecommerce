using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Ecommerce.Pages
{
    public class CategoryContainer : Webpage, IUniquePage
    {
        public virtual int PageSize { get; set; }
        public virtual bool AllowPaging { get; set; }
    }
}