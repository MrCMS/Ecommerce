using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Search;
using MrCMS.Indexing.Management;
using MrCMS.Indexing.Querying;
using MrCMS.Indexing.Utils;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Indexing;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class ProductSearchIndexService : IProductSearchIndexService
    {
        private readonly ISearcher<Product, ProductSearchIndex> _productSearcher;

        public ProductSearchIndexService(ISearcher<Product, ProductSearchIndex> productSearcher)
        {
            _productSearcher = productSearcher;
        }

        public IPagedList<Product> SearchProducts(ProductSearchQuery query)
        {
            IPagedList<Product> searchProducts = _productSearcher.Search(query.GetQuery(), query.Page, query.PageSize, query.GetFilter(), query.GetSort());
            return searchProducts;
        }

        public double GetMaxPrice(ProductSearchQuery query)
        {
            var clone = query.Clone() as ProductSearchQuery;
            clone.PriceTo = null;
            var search = _productSearcher.IndexSearcher.Search(clone.GetQuery(), clone.GetFilter(), int.MaxValue);
            var documents = search.ScoreDocs.Select(doc => _productSearcher.IndexSearcher.Doc(doc.Doc)).ToList();
            var max = documents.Count > 0
                          ? documents.Select(document => document.GetValue<decimal>(FieldDefinition.GetFieldName<ProductSearchPriceDefinition>())).Max()
                          : 0;
            if (documents.Any())
                max = documents.Select(document => document.GetValue<decimal>(FieldDefinition.GetFieldName<ProductSearchPriceDefinition>())).Max();
            return Convert.ToDouble(Math.Ceiling(max / 5.0m) * 5m);
        }

        public List<int> GetSpecifications(ProductSearchQuery query)
        {
            var clone = query.Clone() as ProductSearchQuery;

            var indexSearcher = _productSearcher.IndexSearcher;
            var valueCollector = new ValueCollector(indexSearcher, FieldDefinition.GetFieldName<ProductSearchSpecificationsDefinition>());
            indexSearcher.Search(clone.GetQuery(), clone.GetFilter(), valueCollector);
            return GetSpecifications(valueCollector);
        }

        private static List<int> GetSpecifications(ValueCollector valueCollector)
        {
            return valueCollector.Values[FieldDefinition.GetFieldName<ProductSearchSpecificationsDefinition>()].Select(s => Convert.ToInt32(s)).Distinct().ToList();
        }

        public List<OptionInfo> GetOptions(ProductSearchQuery query)
        {
            var clone = query.Clone() as ProductSearchQuery;
            //clone.Options = new List<string>();
            var indexSearcher = _productSearcher.IndexSearcher;
            var valueCollector = new ValueCollector(indexSearcher, FieldDefinition.GetFieldName<ProductSearchOptionsDefinition>());
            indexSearcher.Search(clone.GetQuery(), clone.GetFilter(), valueCollector);
            return GetOptionInfo(valueCollector);
        }

        private List<OptionInfo> GetOptionInfo(ValueCollector valueCollector)
        {
            return valueCollector.Values[FieldDefinition.GetFieldName<ProductSearchOptionsDefinition>()].Select(GetOptionInfo)
                .Where(info => !info.Equals(default(OptionInfo)))
                .Distinct()
                .ToList();
        }

        private ValueCollector GetOptionValueCollector(ProductSearchQuery query)
        {
            var clone = query.Clone() as ProductSearchQuery;
            //clone.Options = new List<string>();
            var indexSearcher = _productSearcher.IndexSearcher;
            var valueCollector = new ValueCollector(indexSearcher,
                FieldDefinition.GetFieldName<ProductSearchSpecificationsDefinition>(),
                FieldDefinition.GetFieldName<ProductSearchOptionsDefinition>());
            indexSearcher.Search(clone.GetQuery(), clone.GetFilter(), valueCollector);
            return valueCollector;
        }

        public OptionSearchData GetOptionSearchData(ProductSearchQuery query)
        {
            var optionValueCollector = GetOptionValueCollector(query);

            return new OptionSearchData
            {
                Options = GetOptionInfo(optionValueCollector),
                Specifications = GetSpecifications(optionValueCollector)
            };
        }

        private OptionInfo GetOptionInfo(string value)
        {
            if (!value.Contains("["))
                return default(OptionInfo);

            int bracketsOpened = value.IndexOf('[');
            var optionIdString = value.Substring(0, bracketsOpened);

            int startIndex = bracketsOpened + 1;
            var valueData = value.Substring(startIndex, value.Length - startIndex - 1);


            int optionId;
            if (!int.TryParse(optionIdString, out optionId))
                return default(OptionInfo);

            return new OptionInfo { OptionId = optionId, Value = valueData };
        }

        public List<int> GetBrands(ProductSearchQuery query)
        {
            var clone = query.Clone() as ProductSearchQuery;
            clone.BrandId = null;
            var indexSearcher = _productSearcher.IndexSearcher;
            var name = FieldDefinition.GetFieldName<ProductSearchBrandDefinition>();
            var valueCollector = new ValueCollector(indexSearcher, name);
            indexSearcher.Search(clone.GetQuery(), clone.GetFilter(), valueCollector);
            return valueCollector.Values[name].Where(x => !string.IsNullOrEmpty(x)).Select(s => Convert.ToInt32(s)).Distinct().ToList();
        }

        public List<int> GetCategories(ProductSearchQuery query)
        {
            var clone = query.Clone() as ProductSearchQuery;
            clone.CategoryId = null;
            var indexSearcher = _productSearcher.IndexSearcher;
            var name = FieldDefinition.GetFieldName<ProductSearchCategoriesDefinition>();
            var valueCollector = new ValueCollector(indexSearcher, name);
            var query1 = clone.GetQuery();
            indexSearcher.Search(query1, clone.GetFilter(), valueCollector);
            return valueCollector.Values[name].Select(s => Convert.ToInt32(s)).Distinct().ToList();
        }
    }

    public struct OptionInfo
    {
        public int OptionId { get; set; }
        public string Value { get; set; }
    }

    public class ValueCollector : Collector
    {
        private readonly IndexSearcher _indexSearcher;
        private readonly Dictionary<string, List<string>> _values;

        public ValueCollector(IndexSearcher indexSearcher, params string[] fieldNames)
        {
            _indexSearcher = indexSearcher;
            _values = new Dictionary<string, List<string>>();
            foreach (var fieldName in fieldNames)
            {
                _values[fieldName] = new List<string>();
            }
        }

        public override void SetScorer(Scorer scorer) { }
        public override void Collect(int doc)
        {
            var document = _indexSearcher.Doc(doc);
            foreach (var key in Values.Keys)
            {
                Values[key].AddRange(document.GetValues(key));
            }
        }

        public override void SetNextReader(IndexReader reader, int docBase) { }

        public override bool AcceptsDocsOutOfOrder { get { return true; } }

        public Dictionary<string, List<string>> Values
        {
            get { return _values; }
        }
    }
}