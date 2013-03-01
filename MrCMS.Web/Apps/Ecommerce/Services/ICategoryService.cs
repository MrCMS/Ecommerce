using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface ICategoryService
    {
        CategoryPagedList Search(string query, int page);
    }
}