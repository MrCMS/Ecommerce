using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using MrCMS.Entities.Indexes;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using NHibernate;
using Version = Lucene.Net.Util.Version;
using MrCMS.Web.Apps.Ecommerce.Indexing;
using MrCMS.Web.Apps.Ecommerce.Pages;
using Filter = Lucene.Net.Search.Filter;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IProductSearchService
    {
        IPagedList<Product> SearchProducts(ProductSearchQuery query);
        double GetMaxPrice(ProductSearchQuery query);
        List<int> GetSpecifications(ProductSearchQuery query);
        List<int> GetOptions(ProductSearchQuery query);
        List<int> GetBrands(ProductSearchQuery query);
        List<int> GetCategories(ProductSearchQuery query);
    }

    public class ProductSearchQuery : ICloneable
    {
        private double? _maxPrice;

        public ProductSearchQuery()
        {
            Options = new List<int>();
            Specifications = new List<int>();
            Page = 1;
            PageSize = 10;
        }

        public double MaxPrice
        {
            get { return (double)(_maxPrice = _maxPrice ?? MrCMSApplication.Get<IProductSearchService>().GetMaxPrice(this)); }
        }

        public List<int> Options { get; set; }
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
                var options = MrCMSApplication.Get<EcommerceSettings>().SearchProductsPerPage.Split(',').Where(s =>
                {
                    int result;
                    return int.TryParse(s, out result);
                }).Select(s => Convert.ToInt32(s));
                return options.BuildSelectItemList(i => string.Format("{0} products per page", i), i => i.ToString(),
                                                   i => i == PageSize, emptyItem: null);
            }
        }
        public IEnumerable<SelectListItem> SortByOptions
        {
            get
            {
                var productSearchSorts = new List<ProductSearchSort>
                                             {
                                                 ProductSearchSort.MostPopular, ProductSearchSort.Latest, ProductSearchSort.NameAToZ, ProductSearchSort.NameZToA, ProductSearchSort.PriceLowToHigh, ProductSearchSort.PriceHighToLow
                                             };
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                    productSearchSorts.Insert(2, ProductSearchSort.Relevance);
                return productSearchSorts.BuildSelectItemList(sort => sort.GetDescription(), sort => Convert.ToInt32(sort).ToString(),
                                                sort => sort == SortByValue, emptyItem: null);
            }
        }

        public int? BrandId { get; set; }

        public Filter GetFilter()
        {
            var dateValue = DateTools.DateToString(CurrentRequestData.Now, DateTools.Resolution.SECOND);
            var filter = FieldCacheRangeFilter.NewStringRange(ProductSearchIndex.PublishOn.FieldName, null,
                                                              dateValue, false, true);
            return filter;
        }

        public Query GetQuery()
        {
            if (!Options.Any() && !Specifications.Any() && Math.Abs(PriceFrom - 0) < 0.01 && !PriceTo.HasValue && !CategoryId.HasValue && string.IsNullOrWhiteSpace(SearchTerm)
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
                var fuzzySearchTerm = MakeFuzzy(SearchTerm);
                var q = new MultiFieldQueryParser(Version.LUCENE_30, FieldDefinition.GetFieldNames(DocumentIndexDefinition.Name, ProductSearchIndex.SKUs), new StandardAnalyzer(Version.LUCENE_30));
                var query = q.Parse(fuzzySearchTerm);
                booleanQuery.Add(query, Occur.MUST);
            }
            if (BrandId.HasValue)
                booleanQuery.Add(GetBrandQuery(), Occur.MUST);
            return booleanQuery;
        }

        private string MakeFuzzy(string keywords)
        {
            var split = keywords.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", split.Select(s => s + "~"));
        }
        private Query GetOptionsQuery()
        {
            var query = new BooleanQuery();

            foreach (var type in Options)
                query.Add(new TermQuery(new Term(ProductSearchIndex.Options.FieldName, type.ToString())), Occur.MUST);

            return query;
        }

        private Query GetSpecificationsQuery()
        {
            var booleanQuery = new BooleanQuery();

            foreach (var type in Specifications)
                booleanQuery.Add(new TermQuery(new Term(ProductSearchIndex.Specifications.FieldName, type.ToString())),
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
                                               PriceFrom, PriceTo, true, PriceTo.HasValue),
                                           Occur.MUST
                                       }
                                   };
            return booleanQuery;
        }


        private Query GetBrandQuery()
        {
            var booleanQuery = new BooleanQuery();

            booleanQuery.Add(new TermQuery(new Term(ProductSearchIndex.Brand.FieldName, BrandId.ToString())),
                                 Occur.MUST);

            return booleanQuery;
        }

        public Sort GetSort()
        {
            switch (SortByValue)
            {
                case ProductSearchSort.MostPopular:
                    return new Sort(new[] { new SortField(ProductSearchIndex.NumberBought.FieldName, SortField.INT, true) });
                case ProductSearchSort.Latest:
                    return new Sort(new[] { new SortField(ProductSearchIndex.CreatedOn.FieldName, SortField.STRING, true) });
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
                           Specifications = Specifications
                       };
        }
    }

    public enum ProductSearchSort
    {
        [Description("Most Popular")]
        MostPopular = 6,
        [Description("Latest")]
        Latest = 7,
        Relevance = 1,
        [Description("Name A-Z")]
        NameAToZ = 2,
        [Description("Name Z-A")]
        NameZToA = 3,
        [Description("Price Low-High")]
        PriceLowToHigh = 4,
        [Description("Price High-Low")]
        PriceHighToLow = 5,
    }
}