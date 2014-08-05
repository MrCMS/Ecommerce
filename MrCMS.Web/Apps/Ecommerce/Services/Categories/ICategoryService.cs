using System.Collections.Generic;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services.Categories
{
    public interface ICategoryService
    {
        IPagedList<Category> GetCategories(Product product, string query, int page = 1);
        IList<Category> GetSubCategories(Category category);

        CategorySearchModel GetCategoriesForSearch(ProductSearchQuery query);
    }
}