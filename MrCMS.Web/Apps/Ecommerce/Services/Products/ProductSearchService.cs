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
using Lucene.Net.Search;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class ProductSearchService : IProductSearchService
    {
        private readonly ISearcher<Product, ProductSearchIndex> _productSearcher;

        public ProductSearchService(ISearcher<Product, ProductSearchIndex> productSearcher)
        {
            _productSearcher = productSearcher;
        }
        public IPagedList<Product> SearchProducts(string sortBy,List<string> options = null, List<string> specifications = null, decimal priceFrom = 0, decimal priceTo = 0, int page = 1, int pageSize = 10, int categoryId=0)
        {
            var searchQuery = new ProductSearchQuery(options,specifications, priceFrom, priceTo,categoryId);
            Sort sort = null;
            switch (sortBy)
            {
                case "1":
                    sort = new Sort(new[] { SortField.FIELD_SCORE, SortField.FIELD_DOC });
                    break;
                case "2":
                    sort = new Sort(new[] { SortField.FIELD_SCORE, new SortField("nameSort", SortField.STRING) });
                    break;
                case "3":
                    sort = new Sort(new[] { SortField.FIELD_SCORE, new SortField("nameSort", SortField.STRING, true) });
                    break;
                case "4":
                    sort = new Sort(new[] { SortField.FIELD_SCORE, new SortField("price", SortField.STRING, true) });
                    break;
                case "5":
                    sort = new Sort(new[] { SortField.FIELD_SCORE, new SortField("price", SortField.STRING) });
                    break;
            }
            return _productSearcher.Search(searchQuery.GetQuery(), page, pageSize, null, sort);
        }
    }
}