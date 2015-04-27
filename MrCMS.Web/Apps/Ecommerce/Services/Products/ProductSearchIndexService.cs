using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using MrCMS.Indexing.Management;
using MrCMS.Indexing.Querying;
using MrCMS.Indexing.Utils;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Indexing;
using MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using Newtonsoft.Json;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class ProductSearchIndexService : IProductSearchIndexService
    {
        private readonly EcommerceSearchCacheSettings _ecommerceSearchCacheSettings;
        private readonly IGetProductCategories _getProductCategories;
        private readonly IGetProductSearchQueryObjects _getProductSearchQueryObjects;
        private readonly ISearcher<Product, ProductSearchIndex> _productSearcher;

        public ProductSearchIndexService(ISearcher<Product, ProductSearchIndex> productSearcher,
            IGetProductCategories getProductCategories, EcommerceSearchCacheSettings ecommerceSearchCacheSettings,
            IGetProductSearchQueryObjects getProductSearchQueryObjects)
        {
            _productSearcher = productSearcher;
            _getProductCategories = getProductCategories;
            _ecommerceSearchCacheSettings = ecommerceSearchCacheSettings;
            _getProductSearchQueryObjects = getProductSearchQueryObjects;
        }

        public IPagedList<Product> SearchProducts(ProductSearchQuery query)
        {
            IPagedList<Product> searchProducts = _productSearcher.Search(GetQuery(query), query.Page, query.PageSize,
                sort: GetSort(query));
            return searchProducts;
        }

        public double GetMaxPrice(ProductSearchQuery query)
        {
            var clone = query.Clone() as ProductSearchQuery;
            clone.PriceTo = null;
            TopDocs search = _productSearcher.IndexSearcher.Search(GetQuery(clone), int.MaxValue);
            List<Document> documents =
                search.ScoreDocs.Select(doc => _productSearcher.IndexSearcher.Doc(doc.Doc)).ToList();
            decimal max = documents.Count > 0
                ? documents.Select(
                    document => document.GetValue<decimal>(FieldDefinition.GetFieldName<ProductSearchPriceDefinition>()))
                    .Max()
                : 0;
            if (documents.Any())
                max =
                    documents.Select(
                        document =>
                            document.GetValue<decimal>(FieldDefinition.GetFieldName<ProductSearchPriceDefinition>()))
                        .Max();
            return Convert.ToDouble(Math.Ceiling(max / 5.0m) * 5m);
        }

        public List<int> GetSpecifications(ProductSearchQuery query)
        {
            var clone = query.Clone() as ProductSearchQuery;

            IndexSearcher indexSearcher = _productSearcher.IndexSearcher;
            var valueCollector = new ValueCollector(indexSearcher,
                FieldDefinition.GetFieldName<ProductSearchSpecificationsDefinition>());
            indexSearcher.Search(GetQuery(clone), valueCollector);
            return GetSpecifications(valueCollector);
        }

        public List<OptionInfo> GetOptions(ProductSearchQuery query)
        {
            var clone = query.Clone() as ProductSearchQuery;
            IndexSearcher indexSearcher = _productSearcher.IndexSearcher;
            var valueCollector = new ValueCollector(indexSearcher,
                FieldDefinition.GetFieldName<ProductSearchOptionsDefinition>());
            indexSearcher.Search(GetQuery(clone), valueCollector);
            return GetOptionInfo(valueCollector);
        }

        public OptionSearchData GetOptionSearchData(ProductSearchQuery query)
        {
            ValueCollector optionValueCollector = GetOptionValueCollector(query);

            return new OptionSearchData
            {
                Options = GetOptionInfo(optionValueCollector),
                Specifications = GetSpecifications(optionValueCollector)
            };
        }

        public List<int> GetBrands(ProductSearchQuery query)
        {
            var clone = query.Clone() as ProductSearchQuery;
            clone.BrandId = null;
            IndexSearcher indexSearcher = _productSearcher.IndexSearcher;
            string name = FieldDefinition.GetFieldName<ProductSearchBrandDefinition>();
            var valueCollector = new ValueCollector(indexSearcher, name);
            indexSearcher.Search(GetQuery(clone), valueCollector);
            return
                valueCollector.Values[name].Where(x => !string.IsNullOrEmpty(x))
                    .Select(s => Convert.ToInt32(s))
                    .Distinct()
                    .ToList();
        }

        public List<int> GetCategories(ProductSearchQuery query)
        {
            var clone = query.Clone() as ProductSearchQuery;
            clone.CategoryId = null;
            Query searchQuery = GetQuery(clone);
            return _getProductCategories.Get(searchQuery);
        }

        public CachingInfo GetCachingInfo(ProductSearchQuery query, string suffix = null)
        {
            return new CachingInfo(_ecommerceSearchCacheSettings.SearchCache, GetCacheKey(query) + suffix,
                TimeSpan.FromSeconds(_ecommerceSearchCacheSettings.SearchCacheLength),
                _ecommerceSearchCacheSettings.SearchCacheExpiryType);
        }

        private Sort GetSort(ProductSearchQuery query)
        {
            return _getProductSearchQueryObjects.GetSort(query);
        }
        private Query GetQuery(ProductSearchQuery query)
        {
            return _getProductSearchQueryObjects.GetQuery(query);
        }

        private static List<int> GetSpecifications(ValueCollector valueCollector)
        {
            return
                valueCollector.Values[FieldDefinition.GetFieldName<ProductSearchSpecificationsDefinition>()].Select(
                    s => Convert.ToInt32(s)).Distinct().ToList();
        }

        private List<OptionInfo> GetOptionInfo(ValueCollector valueCollector)
        {
            return
                valueCollector.Values[FieldDefinition.GetFieldName<ProductSearchOptionsDefinition>()].Select(
                    GetOptionInfo)
                    .Where(info => !info.Equals(default(OptionInfo)))
                    .Distinct()
                    .ToList();
        }

        private ValueCollector GetOptionValueCollector(ProductSearchQuery query)
        {
            var clone = query.Clone() as ProductSearchQuery;
            //clone.Options = new List<string>();
            IndexSearcher indexSearcher = _productSearcher.IndexSearcher;
            var valueCollector = new ValueCollector(indexSearcher,
                FieldDefinition.GetFieldName<ProductSearchSpecificationsDefinition>(),
                FieldDefinition.GetFieldName<ProductSearchOptionsDefinition>());
            indexSearcher.Search(GetQuery(clone), valueCollector);
            return valueCollector;
        }

        private OptionInfo GetOptionInfo(string value)
        {
            if (!value.Contains("["))
                return default(OptionInfo);

            int bracketsOpened = value.IndexOf('[');
            string optionIdString = value.Substring(0, bracketsOpened);

            int startIndex = bracketsOpened + 1;
            string valueData = value.Substring(startIndex, value.Length - startIndex - 1);


            int optionId;
            if (!int.TryParse(optionIdString, out optionId))
                return default(OptionInfo);

            return new OptionInfo { OptionId = optionId, Value = valueData };
        }

        private string GetCacheKey(ProductSearchQuery query)
        {
            return _ecommerceSearchCacheSettings.SearchCachePerUser
                ? JsonConvert.SerializeObject(new { CurrentRequestData.UserGuid, query })
                : JsonConvert.SerializeObject(query);
        }
    }
}