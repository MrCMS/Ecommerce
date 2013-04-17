using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IProductService
    {
        ProductPagedList Search(string queryTerm = null, int page = 1);
        void MakeMultiVariant(Product product, string option1, string option2, string option3);
        void AddCategory(Product product, int categoryId);
        void RemoveCategory(Product product, int categoryId);
        List<SelectListItem> GetOptions();
        Product Get(int id);
    }
}