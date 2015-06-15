using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IBrandListingUIService
    {
        IPagedList<Brand> GetBrands(int pageNumber, int pageSize);
    }
}