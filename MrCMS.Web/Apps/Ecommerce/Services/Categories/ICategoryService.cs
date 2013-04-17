using System.Collections.Generic;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Services.Categories
{
    public interface ICategoryService
    {
        CategoryPagedList Search(string query = null, int page = 1);
        IEnumerable<AutoCompleteResult> Search(string query, List<int> ids);
        IPagedList<Category> GetCategories(Product product, string query, int page);
    }
}