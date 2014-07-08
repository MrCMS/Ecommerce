using System.Collections.Generic;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IProductSearchIndexService
    {
        IPagedList<Product> SearchProducts(ProductSearchQuery query);
        double GetMaxPrice(ProductSearchQuery query);
        List<int> GetSpecifications(ProductSearchQuery query);
        List<OptionInfo> GetOptions(ProductSearchQuery query);
        OptionSearchData GetOptionSearchData(ProductSearchQuery query);
        List<int> GetBrands(ProductSearchQuery query);
        List<int> GetCategories(ProductSearchQuery query);
    }
}