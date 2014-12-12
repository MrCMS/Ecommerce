using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Search;
using MrCMS.Indexing.Management;
using MrCMS.Indexing.Querying;
using MrCMS.Web.Apps.Ecommerce.Indexing;
using MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class GetProductCategories : IGetProductCategories
    {
        private readonly ISearcher<Product, ProductSearchIndex> _productSearcher;

        public GetProductCategories(ISearcher<Product, ProductSearchIndex> productSearcher)
        {
            _productSearcher = productSearcher;
        }

        public List<int> Get(Query searchQuery, Filter filter)
        {
            var indexSearcher = _productSearcher.IndexSearcher;
            var name = FieldDefinition.GetFieldName<ProductSearchCategoriesDefinition>();
            var valueCollector = new ValueCollector(indexSearcher, name);
            indexSearcher.Search(searchQuery, filter, valueCollector);
            return valueCollector.Values[name].Select(s => Convert.ToInt32((string) s)).Distinct().ToList();
        }
    }
}