using System.Collections.Generic;
using Lucene.Net.Search;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IGetProductCategories
    {
        List<int> Get(Query searchQuery, Filter filter);
    }
}