using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Management;
using MrCMS.Indexing.Querying;
using MrCMS.Web.Apps.Core.Indexing;
using MrCMS.Web.Apps.Core.Indexing.WebpageSearch;
using MrCMS.Web.Apps.Ecommerce.Indexing;
using MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class GetProductCategories : IGetProductCategories
    {
        private readonly ISearcher<Product, ProductSearchIndex> _productSearcher;
        private readonly ISearcher<Webpage, WebpageSearchIndexDefinition> _indexSearcher;

        public GetProductCategories(ISearcher<Product, ProductSearchIndex> productSearcher,
            ISearcher<Webpage, WebpageSearchIndexDefinition> indexSearcher)
        {
            _productSearcher = productSearcher;
            _indexSearcher = indexSearcher;
        }

        public List<int> Get(Query searchQuery, Filter filter)
        {
            var indexSearcher = _productSearcher.IndexSearcher;
            var name = FieldDefinition.GetFieldName<ProductSearchCategoriesDefinition>();
            var valueCollector = new ValueCollector(indexSearcher, name);
            indexSearcher.Search(searchQuery, filter, valueCollector);
            var categoryIds = valueCollector.Values[name].Select(s => Convert.ToInt32(s)).Distinct().ToList();

            if (!categoryIds.Any())
                return categoryIds;

            var mainQuery = new BooleanQuery();
            var idsQuery = new BooleanQuery();
            const string idFieldName = "id";
            foreach (var categoryId in categoryIds)
            {
                idsQuery.Add(new TermQuery(new Term(idFieldName, categoryId.ToString())), Occur.SHOULD);
            }
            mainQuery.Add(idsQuery, Occur.MUST);

            var publishedOn = FieldDefinition.GetFieldName<PublishedOnFieldDefinition>();
            mainQuery.Add(new TermRangeQuery(
                                           publishedOn, null,
                                           DateTools.DateToString(CurrentRequestData.Now, DateTools.Resolution.SECOND), false, true), Occur.MUST);
            var webpageSearcher = _indexSearcher.IndexSearcher;
            var webpageValueCollector = new ValueCollector(webpageSearcher, idFieldName);
            webpageSearcher.Search(mainQuery, null, webpageValueCollector);

            return webpageValueCollector.Values[idFieldName].Select(s => Convert.ToInt32(s))
                .Intersect(categoryIds)
                .ToList();
        }
    }
}