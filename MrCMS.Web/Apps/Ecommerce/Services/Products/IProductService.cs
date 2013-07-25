using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IProductService
    {
        ProductPagedList Search(string queryTerm = null, int page = 1);
        IList<Product> Search(string queryTerm);
        void AddCategory(Product product, int categoryId);
        void RemoveCategory(Product product, int categoryId);
        List<SelectListItem> GetOptions();
        Product Get(int id);
        Product GetByName(string name);
        Product GetByUrl(string url);
        IList<Product> GetAll();
        void SetCategoryOrder(Product product, List<SortItem> items);
    }
}