using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using MrCMS.Entities.Indexes;
using MrCMS.Indexing.Management;
using MrCMS.Paging;
using MrCMS.Website;
using Version = Lucene.Net.Util.Version;
using MrCMS.Web.Apps.Ecommerce.Indexing;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IProductSearchService
    {
        IPagedList<Product> SearchProducts(string searchTerm, string sortBy, List<string> options = null, List<string> specifications = null, decimal priceFrom = 0, decimal priceTo = 0, int page = 1, int pageSize = 10, int categoryId = 0);
    }

    public class ProductSearchQuery : ICloneable
    {
        public List<string> Options { get; set; }
        public List<string> Specifications { get; set; }
        public double PriceFrom { get; set; }
        public double PriceTo { get; set; }
        public int CategoryId { get; set; }
        public string SearchTerm { get; set; }

        public ProductSearchQuery(string searchTerm = "", List<string> options = null, List<string> specifications = null, decimal priceFrom = 0, decimal priceTo = 0, int categoryId = 0)
        {
            if (categoryId != 0)
                CategoryId = categoryId;
            Options = options ?? new List<string>();
            Specifications = specifications ?? new List<string>();
            PriceFrom = Double.Parse(priceFrom.ToString());
            PriceTo = Double.Parse(priceTo.ToString());
            SearchTerm = searchTerm;
        }

        public Filter GetFilter()
        {
            var dateValue = DateTools.DateToString(CurrentRequestData.Now, DateTools.Resolution.SECOND);
            var filter = FieldCacheRangeFilter.NewStringRange(ProductSearchIndex.PublishOn.FieldName, null,
                                                              dateValue, false, true);
            return filter;
        }

        public Query GetQuery()
        {
            if (!Options.Any() && !Specifications.Any() && PriceFrom == 0 && PriceTo == 0 && CategoryId == 0 && String.IsNullOrWhiteSpace(SearchTerm))
                return new MatchAllDocsQuery();

            var booleanQuery = new BooleanQuery();
            if (Options.Any())
                booleanQuery.Add(GetOptionsQuery(), Occur.MUST);
            if (Specifications.Any())
                booleanQuery.Add(GetSpecificationsQuery(), Occur.MUST);
            if (PriceFrom >= 0 && PriceTo != 0)
                booleanQuery.Add(GetPriceRangeQuery(), Occur.MUST);
            if (PriceFrom >= 0 && PriceTo != 0)
                booleanQuery.Add(GetPriceRangeQuery(), Occur.MUST);
            if (CategoryId != 0)
                booleanQuery.Add(GetCategoriesQuery(), Occur.MUST);
            if (!String.IsNullOrWhiteSpace(SearchTerm))
            {
                var fuzzySearchTerm = MakeFuzzy(SearchTerm);
                var q = new MultiFieldQueryParser(Version.LUCENE_30, FieldDefinition.GetFieldNames(
                                                                          DocumentIndexDefinition.Name), new StandardAnalyzer(Version.LUCENE_30));
                var query = q.Parse(fuzzySearchTerm);
                booleanQuery.Add(query, Occur.MUST);
            }
            return booleanQuery;
        }
        private string MakeFuzzy(string keywords)
        {
            var split = keywords.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", split.Select(s => s + "~"));
        }
        private Query GetOptionsQuery()
        {
            BooleanQuery query = new BooleanQuery();

            foreach (var type in Options)
                query.Add(new TermQuery(new Term(ProductSearchIndex.Options.FieldName, type)), Occur.MUST);

            return query;
        }

        private Query GetSpecificationsQuery()
        {
            var booleanQuery = new BooleanQuery();

            foreach (var type in Specifications)
                booleanQuery.Add(new TermQuery(new Term(ProductSearchIndex.Specifications.FieldName, type)),
                                 Occur.MUST);

            return booleanQuery;
        }

        private Query GetCategoriesQuery()
        {
            var booleanQuery = new BooleanQuery();

            booleanQuery.Add(new TermQuery(new Term(ProductSearchIndex.Categories.FieldName, CategoryId.ToString())),
                                 Occur.MUST);

            return booleanQuery;
        }

        private Query GetPriceRangeQuery()
        {
            var booleanQuery = new BooleanQuery
                                   {
                                       {
                                           NumericRangeQuery.NewDoubleRange(
                                               ProductSearchIndex.Price.FieldName,
                                               PriceFrom, PriceTo, true, true),
                                           Occur.MUST
                                       }
                                   };
            return booleanQuery;
        }

        public object Clone()
        {
            return new ProductSearchQuery
            {
                SearchTerm = SearchTerm,
                Specifications = Specifications,
                Options = Options,
                PriceFrom = PriceFrom,
                PriceTo = PriceTo,
                CategoryId = CategoryId
            };
        }
    }
}