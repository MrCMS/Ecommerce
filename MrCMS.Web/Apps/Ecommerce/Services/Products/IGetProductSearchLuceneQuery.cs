using Lucene.Net.Search;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IGetProductSearchLuceneQuery
    {
        Query Get(ProductSearchQuery searchQuery);
    }
}