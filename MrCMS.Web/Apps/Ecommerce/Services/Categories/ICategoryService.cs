using System.Collections.Generic;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Services.Categories
{
    public interface ICategoryService
    {
        IPagedList<Category> GetCategories(Product product, string query, int page = 1);
        IList<Category> GetSubCategories(Category category);
        Category GetCategory(int id);

        CategorySearchModel GetCategoriesForSearch(ProductSearchQuery query);
    }
}