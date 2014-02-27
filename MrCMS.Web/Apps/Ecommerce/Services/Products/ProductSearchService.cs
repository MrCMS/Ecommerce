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

        public IPagedList<Product> SearchProducts(ProductSearchQuery query)
        {
            return _productSearcher.Search(query.GetQuery(), query.Page, query.PageSize, query.GetFilter(), query.GetSort());
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
            return valueCollector.Values.Select(s => Convert.ToInt32(s)).Distinct().ToList();
        }

        public List<int> GetOptions(ProductSearchQuery query)
        {
            var clone = query.Clone() as ProductSearchQuery;
            clone.Options = new List<int>();
            var indexSearcher = _productSearcher.IndexSearcher;
            var valueCollector = new ValueCollector(indexSearcher, FieldDefinition.GetFieldName<ProductSearchOptionsDefinition>());
            indexSearcher.Search(clone.GetQuery(), clone.GetFilter(), valueCollector);
            return valueCollector.Values.Select(s => Convert.ToInt32(s)).Distinct().ToList();
        }

        public List<int> GetBrands(ProductSearchQuery query)
        {
            var clone = query.Clone() as ProductSearchQuery;
            clone.BrandId = null;
            var indexSearcher = _productSearcher.IndexSearcher;
            var valueCollector = new ValueCollector(indexSearcher, FieldDefinition.GetFieldName<ProductSearchBrandDefinition>());
            indexSearcher.Search(clone.GetQuery(), clone.GetFilter(), valueCollector);
            return valueCollector.Values.Select(s => Convert.ToInt32(s)).Distinct().ToList();
        }

        public List<int> GetCategories(ProductSearchQuery query)
        {
            var clone = query.Clone() as ProductSearchQuery;
            clone.CategoryId = null;
            var indexSearcher = _productSearcher.IndexSearcher;
            var valueCollector = new ValueCollector(indexSearcher, FieldDefinition.GetFieldName<ProductSearchCategoriesDefinition>());
            indexSearcher.Search(clone.GetQuery(), clone.GetFilter(), valueCollector);
            return valueCollector.Values.Select(s => Convert.ToInt32(s)).Distinct().ToList();
        }
    }

    public class ValueCollector : Collector
    {
        private readonly IndexSearcher _indexSearcher;
        private readonly string _fieldName;
        private readonly List<string> _values = new List<string>();

        public ValueCollector(IndexSearcher indexSearcher, string fieldName)
        {
            _indexSearcher = indexSearcher;
            _fieldName = fieldName;
        }

        public override void SetScorer(Scorer scorer) { }
        public override void Collect(int doc)
        {
            var strings = _indexSearcher.Doc(doc).GetValues(_fieldName);
            Values.AddRange(strings);
        }

        public override void SetNextReader(IndexReader reader, int docBase) { }

        public override bool AcceptsDocsOutOfOrder { get { return true; } }

        public List<string> Values
        {
            get { return _values; }
        }
    }
}