using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using MrCMS.Helpers;
using MrCMS.Indexing;
using MrCMS.Indexing.Management;
using MrCMS.Indexing.Utils;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Indexing;
using MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using NHibernate;
using Filter = Lucene.Net.Search.Filter;
using Version = Lucene.Net.Util.Version;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class ProductSearchQuery : ICloneable
    {
        private double? _maxPrice;

        public ProductSearchQuery()
        {
            Options = new List<string>();
            Specifications = new List<int>();
            Page = 1;
            PageSize = 10;
        }

        public double MaxPrice
        {
            get
            {
                return
                    (double)(_maxPrice = _maxPrice ?? MrCMSApplication.Get<IProductSearchIndexService>().GetMaxPrice(this));
            }
        }

        public List<string> Options { get; set; }
        public List<int> Specifications { get; set; }
        public double PriceFrom { get; set; }
        public double? PriceTo { get; set; }
        public int? CategoryId { get; set; }
        public string SearchTerm { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public ProductSearchSort? SortBy { get; set; }

        public IEnumerable<SelectListItem> PerPageOptions
        {
            get
            {
                IEnumerable<int> options = MrCMSApplication.Get<EcommerceSettings>()
                    .SearchProductsPerPage.Split(',')
                    .Where(s =>
                    {
                        int result;
                        return int.TryParse(s, out result);
                    }).Select(s => Convert.ToInt32(s));
                return options.BuildSelectItemList(i => string.Format((string)"{0} products per page", (object)i), i => i.ToString(),
                    i => i == PageSize, emptyItem: null);
            }
        }

        public IEnumerable<SelectListItem> SortByOptions
        {
            get
            {
                var productSearchSorts = new List<ProductSearchSort>
                {
                    ProductSearchSort.MostPopular,
                    ProductSearchSort.Latest,
                    ProductSearchSort.NameAToZ,
                    ProductSearchSort.NameZToA,
                    ProductSearchSort.PriceLowToHigh,
                    ProductSearchSort.PriceHighToLow
                };
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                    productSearchSorts.Insert(0, ProductSearchSort.Relevance);
                return productSearchSorts.BuildSelectItemList(sort => sort.GetDescription(),
                    sort => Convert.ToInt32(sort).ToString(),
                    sort => sort == SortByValue, emptyItem: null);
            }
        }

        public int? BrandId { get; set; }

        private ProductSearchSort SortByValue
        {
            get
            {
                if (SortBy.HasValue)
                    return SortBy.Value;
                if (CategoryId.HasValue)
                {
                    var category = MrCMSApplication.Get<ISession>().Get<Category>(CategoryId);
                    if (category != null && category.DefaultProductSearchSort.HasValue)
                        return category.DefaultProductSearchSort.Value;
                }
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                    return ProductSearchSort.Relevance;
                return ProductSearchSort.MostPopular;
            }
        }

        public object Clone()
        {
            return new ProductSearchQuery
            {
                CategoryId = CategoryId,
                Options = Options,
                Page = Page,
                PageSize = PageSize,
                PriceFrom = PriceFrom,
                PriceTo = PriceTo,
                SearchTerm = SearchTerm,
                SortBy = SortBy,
                Specifications = Specifications,
                BrandId = BrandId
            };
        }

        public Filter GetFilter()
        {
            string dateValue = DateTools.DateToString(CurrentRequestData.Now, DateTools.Resolution.SECOND);
            FieldCacheRangeFilter<string> filter =
                FieldCacheRangeFilter.NewStringRange(FieldDefinition.GetFieldName<ProductSearchPublishOnDefinition>(),
                    null,
                    dateValue, false, true);
            return filter;
        }

        public Query GetQuery()
        {
            if (!Options.Any() && !Specifications.Any() && Math.Abs(PriceFrom - 0) < 0.01 && !PriceTo.HasValue &&
                !CategoryId.HasValue && string.IsNullOrWhiteSpace(SearchTerm)
                && !BrandId.HasValue)
                return new MatchAllDocsQuery();

            var booleanQuery = new BooleanQuery();
            if (Options.Any())
                booleanQuery.Add(GetOptionsQuery(), Occur.MUST);
            if (Specifications.Any())
                booleanQuery.Add(GetSpecificationsQuery(), Occur.MUST);
            if (CategoryId.HasValue)
                booleanQuery.Add(GetCategoriesQuery(), Occur.MUST);
            if (PriceFrom > 0 || PriceTo.HasValue)
                booleanQuery.Add(GetPriceRangeQuery(), Occur.MUST);
            if (!String.IsNullOrWhiteSpace(SearchTerm))
            {
                IndexDefinition indexDefinition = IndexingHelper.Get<ProductSearchIndex>();
                Analyzer analyser = indexDefinition.GetAnalyser();
                var parser = new MultiFieldQueryParser(Version.LUCENE_30, indexDefinition.SearchableFieldNames, analyser);
                Query query = SearchTerm.SafeGetSearchQuery(parser, analyser);

                booleanQuery.Add(query, Occur.MUST);
            }
            if (BrandId.HasValue)
                booleanQuery.Add(GetBrandQuery(), Occur.MUST);
            return booleanQuery;
        }

        private Query GetOptionsQuery()
        {
            var query = new BooleanQuery();

            foreach (string type in Options)
                query.Add(
                    new TermQuery(new Term(FieldDefinition.GetFieldName<ProductSearchOptionsDefinition>(), type)),
                    Occur.MUST);

            return query;
        }

        private Query GetSpecificationsQuery()
        {
            var booleanQuery = new BooleanQuery();

            foreach (int type in Specifications)
                booleanQuery.Add(
                    new TermQuery(new Term(FieldDefinition.GetFieldName<ProductSearchSpecificationsDefinition>(),
                        type.ToString())),
                    Occur.MUST);

            return booleanQuery;
        }

        private Query GetCategoriesQuery()
        {
            var booleanQuery = new BooleanQuery();

            booleanQuery.Add(
                new TermQuery(new Term(FieldDefinition.GetFieldName<ProductSearchCategoriesDefinition>(),
                    CategoryId.ToString())),
                Occur.MUST);

            return booleanQuery;
        }

        private Query GetPriceRangeQuery()
        {
            var booleanQuery = new BooleanQuery
            {
                {
                    NumericRangeQuery.NewDoubleRange(
                        FieldDefinition.GetFieldName<ProductSearchPriceDefinition>(),
                        PriceFrom, PriceTo, true, PriceTo.HasValue),
                    Occur.MUST
                }
            };
            return booleanQuery;
        }


        private Query GetBrandQuery()
        {
            var booleanQuery = new BooleanQuery();

            booleanQuery.Add(
                new TermQuery(new Term(FieldDefinition.GetFieldName<ProductSearchBrandDefinition>(), BrandId.ToString())),
                Occur.MUST);

            return booleanQuery;
        }

        public Sort GetSort()
        {
            switch (SortByValue)
            {
                case ProductSearchSort.MostPopular:
                    return
                        new Sort(new[]
                        {
                            new SortField(FieldDefinition.GetFieldName<ProductSearchNumberBoughtDefinition>(),
                                SortField.INT, true)
                        });
                case ProductSearchSort.Latest:
                    return
                        new Sort(new[]
                        {
                            new SortField(FieldDefinition.GetFieldName<ProductSearchCreatedOnDefinition>(),
                                SortField.STRING, true)
                        });
                case ProductSearchSort.NameAToZ:
                    return new Sort(new[] { new SortField("nameSort", SortField.STRING) });
                case ProductSearchSort.NameZToA:
                    return new Sort(new[] { new SortField("nameSort", SortField.STRING, true) });
                case ProductSearchSort.PriceLowToHigh:
                    return new Sort(new[] { new SortField("price", SortField.DOUBLE) });
                case ProductSearchSort.PriceHighToLow:
                    return new Sort(new[] { new SortField("price", SortField.DOUBLE, true) });
                default:
                    return Sort.RELEVANCE;
            }
        }
    }
}