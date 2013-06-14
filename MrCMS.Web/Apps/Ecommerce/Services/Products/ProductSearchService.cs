using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Helpers;
using MrCMS.Indexing.Querying;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Indexing;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class ProductSearchService : IProductSearchService
    {
        private readonly ISearcher<Product, ProductSearchIndex> _productSearcher;

        public ProductSearchService(ISearcher<Product, ProductSearchIndex> productSearcher)
        {
            _productSearcher = productSearcher;
        }
        public IPagedList<Product> SearchProducts(List<string> options = null, List<string> specifications = null, decimal priceFrom = 0, decimal priceTo = 0, int page = 1, int pageSize = 10)
        {
            var searchQuery = new ProductSearchQuery(options,specifications, priceFrom, priceTo);
            return _productSearcher.Search(searchQuery.GetQuery(), page, pageSize, null);
        }
    }
}