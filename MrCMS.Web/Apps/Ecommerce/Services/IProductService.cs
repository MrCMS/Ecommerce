using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IProductService
    {
        IPagedList<Product> Search(string queryTerm = null, int page = 1);
    }
}