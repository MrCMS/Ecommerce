using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IProductService
    {
        ProductVariantPagedList Search(string queryTerm = null, int page = 1);
        IList<Product> Search(string queryTerm);
        IPagedList<Product> RelatedProductsSearch(Product product, string query, int page = 1);
        void AddCategory(Product product, int categoryId);
        void RemoveCategory(Product product, int categoryId);
        void AddRelatedProduct(Product product, int relatedProductId);
        void RemoveRelatedProduct(Product product, int relatedProductId);
        List<SelectListItem> GetOptions();
        Product Get(int id);
        Product GetByName(string name);
        Product GetByUrl(string url);
        IList<Product> GetAll();
        IList<Product> GetNewIn(int numberOfItems = 10);
        void SetCategoryOrder(Product product, List<SortItem> items);
        Product Update(Product product);
        void SetVariantOrders(Product product, List<SortItem> items);

        List<SelectListItem> GetPublishStatusOptions();

        IPagedList<Product> Search(ProductAdminSearchQuery query);
    }
}