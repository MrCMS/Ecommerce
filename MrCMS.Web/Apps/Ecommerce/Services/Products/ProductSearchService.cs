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
using NHibernate;
using MrCMS.Entities.Multisite;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class ProductSearchService : IProductSearchService
    {
        private readonly ISession _session;
        private readonly ISearcher<Product, ProductSearchIndex> _productSearcher;
         private readonly CurrentSite _currentSite;

         public ProductSearchService(ISession session, CurrentSite currentSite, ISearcher<Product, ProductSearchIndex> productSearcher)
        {
            _session = session;
            _currentSite = currentSite;
            _productSearcher = productSearcher;
        }
        public IPagedList<Product> SearchProducts(string searchTerm,string sortBy,List<string> options = null, List<string> specifications = null, decimal priceFrom = 0, decimal priceTo = 0, int page = 1, int pageSize = 10, int categoryId=0)
        {
            var searchQuery = new ProductSearchQuery(searchTerm,options, specifications, priceFrom, priceTo, categoryId);
            Sort sort = null;
            switch (sortBy)
            {
                case "1":
                    sort = new Sort(new SortField[] { SortField.FIELD_SCORE, SortField.FIELD_DOC});
                    break;
                case "2":
                    sort = new Sort(new SortField[] {SortField.FIELD_SCORE, new SortField("nameSort", SortField.STRING)});
                    break;
                case "3":
                    sort = new Sort(new SortField[] { SortField.FIELD_SCORE, new SortField("nameSort", SortField.STRING, true) });
                    break;
                case "4":
                    sort = new Sort(new SortField[] { SortField.FIELD_SCORE, new SortField("price", SortField.STRING, true) });
                    break;
                case "5":
                    sort = new Sort(new SortField[] { SortField.FIELD_SCORE, new SortField("price", SortField.STRING) });
                    break;
                default:
                    break;
            }
            if(sort!=null)
                return _productSearcher.SearchWithSort(searchQuery.GetQuery(), page, pageSize, sort, null);
            else
                return _productSearcher.Search(searchQuery.GetQuery(), page, pageSize, null);
        }
        public ProductSearch GetSiteProductSearch()
        {
            IList<ProductSearch> productSearchers = _session.QueryOver<ProductSearch>().Where(x => x.Site == _currentSite.Site).Cacheable().List();
            if (productSearchers.Any())
                return _session.QueryOver<ProductSearch>().Cacheable().List().First();
            else
                return null;
        }
    }
}