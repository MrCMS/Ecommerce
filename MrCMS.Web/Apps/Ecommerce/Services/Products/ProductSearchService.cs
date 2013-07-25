using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Indexing.Querying;
using MrCMS.Indexing.Utils;
using MrCMS.Paging;
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
            decimal max = 500m;
            if (documents.Any())
                max = documents.Select(document => document.GetValue<decimal>(ProductSearchIndex.Price.FieldName)).Max();
            return Convert.ToDouble(Math.Ceiling(max/5.0m)*5m);
        }

        public List<int> GetSpecifications(ProductSearchQuery query)
        {
            var clone = query.Clone() as ProductSearchQuery;
            clone.Specifications = new List<int>();
            var search = _productSearcher.IndexSearcher.Search(clone.GetQuery(), clone.GetFilter(), int.MaxValue);
            var documents = search.ScoreDocs.Select(doc => _productSearcher.IndexSearcher.Doc(doc.Doc)).ToList();
            return
                documents.SelectMany(
                    document =>
                    document.GetValues(ProductSearchIndex.Specifications.FieldName).Select(s => Convert.ToInt32(s))).Distinct()
                         .ToList();
        }

        public List<int> GetOptions(ProductSearchQuery query)
        {
            var clone = query.Clone() as ProductSearchQuery;
            clone.Specifications = new List<int>();
            var search = _productSearcher.IndexSearcher.Search(clone.GetQuery(), clone.GetFilter(), int.MaxValue);
            var documents = search.ScoreDocs.Select(doc => _productSearcher.IndexSearcher.Doc(doc.Doc)).ToList();
            return
                documents.SelectMany(
                    document =>
                    document.GetValues(ProductSearchIndex.Options.FieldName).Select(s => Convert.ToInt32(s))).Distinct()
                         .ToList();
        }

        public List<int> GetBrands(ProductSearchQuery query)
        {
            var clone = query.Clone() as ProductSearchQuery;
            clone.BrandId = null;
            var search = _productSearcher.IndexSearcher.Search(clone.GetQuery(), clone.GetFilter(), int.MaxValue);
            var documents = search.ScoreDocs.Select(doc => _productSearcher.IndexSearcher.Doc(doc.Doc)).ToList();
            return
                documents.SelectMany(
                    document =>
                    document.GetValues(ProductSearchIndex.Brand.FieldName).Select(s => Convert.ToInt32(s))).Distinct()
                         .ToList();
        }
    }
}