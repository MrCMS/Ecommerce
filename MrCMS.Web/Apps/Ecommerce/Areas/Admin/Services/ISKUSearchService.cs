using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface ISKUSearchService
    {
        IPagedList<ProductVariant> Search(string query, string skus, int page);
    }
}