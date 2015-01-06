using Lucene.Net.Search;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IGetProductSearchQueryObjects
    {
        Query GetQuery(ProductSearchQuery searchQuery);
        Filter GetFilter(ProductSearchQuery query);
        Sort GetSort(ProductSearchQuery query);
    }
}
