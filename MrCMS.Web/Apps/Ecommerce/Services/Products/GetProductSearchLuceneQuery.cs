using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using MrCMS.Indexing;
using MrCMS.Indexing.Management;
using MrCMS.Indexing.Utils;
using MrCMS.Web.Apps.Ecommerce.Indexing;
using MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions;
using MrCMS.Web.Apps.Ecommerce.Models;
using Version = Lucene.Net.Util.Version;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class GetProductSearchLuceneQuery : IGetProductSearchLuceneQuery
    {
        public Query Get(ProductSearchQuery searchQuery)
        {
            if (!searchQuery.Options.Any() && !searchQuery.Specifications.Any() && Math.Abs(searchQuery.PriceFrom - 0) < 0.01 && !searchQuery.PriceTo.HasValue &&
                !searchQuery.CategoryId.HasValue && string.IsNullOrWhiteSpace(searchQuery.SearchTerm)
                && !searchQuery.BrandId.HasValue)
                return new MatchAllDocsQuery();

            var booleanQuery = new BooleanQuery();
            if (searchQuery.Options.Any())
                booleanQuery.Add(GetOptionsQuery(searchQuery.Options), Occur.MUST);
            if (searchQuery.Specifications.Any())
                booleanQuery.Add(GetSpecificationsQuery(searchQuery.Specifications), Occur.MUST);
            if (searchQuery.CategoryId.HasValue)
                booleanQuery.Add(GetCategoriesQuery(searchQuery.CategoryId.Value), Occur.MUST);
            if (searchQuery.PriceFrom > 0 ||searchQuery.PriceTo.HasValue)
                booleanQuery.Add(GetPriceRangeQuery(searchQuery), Occur.MUST);
            if (!String.IsNullOrWhiteSpace(searchQuery.SearchTerm))
            {
                IndexDefinition indexDefinition = IndexingHelper.Get<ProductSearchIndex>();
                Analyzer analyser = indexDefinition.GetAnalyser();
                var parser = new MultiFieldQueryParser(Version.LUCENE_30, indexDefinition.SearchableFieldNames, analyser);
                Query query = searchQuery.SearchTerm.SafeGetSearchQuery(parser, analyser);

                booleanQuery.Add(query, Occur.MUST);
            }
            if (searchQuery.BrandId.HasValue)
                booleanQuery.Add(GetBrandQuery(searchQuery.BrandId.Value), Occur.MUST);
            return booleanQuery;
        }

        private Query GetOptionsQuery(List<string> options)
        {
            var query = new BooleanQuery();

            foreach (string type in options)
                query.Add(
                    new TermQuery(new Term(FieldDefinition.GetFieldName<ProductSearchOptionsDefinition>(), type)),
                    Occur.MUST);

            return query;
        }

        private Query GetSpecificationsQuery(List<int> specifications)
        {
            var booleanQuery = new BooleanQuery();

            foreach (int type in specifications)
                booleanQuery.Add(
                    new TermQuery(new Term(FieldDefinition.GetFieldName<ProductSearchSpecificationsDefinition>(),
                        type.ToString())),
                    Occur.MUST);

            return booleanQuery;
        }

        private Query GetCategoriesQuery(int categoryId)
        {
            var booleanQuery = new BooleanQuery();

            booleanQuery.Add(
                new TermQuery(new Term(FieldDefinition.GetFieldName<ProductSearchCategoriesDefinition>(),
                    categoryId.ToString())),
                Occur.MUST);

            return booleanQuery;
        }

        private Query GetPriceRangeQuery(ProductSearchQuery searchQuery)
        {
            var booleanQuery = new BooleanQuery
            {
                {
                    NumericRangeQuery.NewDoubleRange(
                        FieldDefinition.GetFieldName<ProductSearchPriceDefinition>(),
                        searchQuery.PriceFrom, searchQuery.PriceTo, true, searchQuery.PriceTo.HasValue),
                    Occur.MUST
                }
            };
            return booleanQuery;
        }


        private Query GetBrandQuery(int brandId)
        {
            var booleanQuery = new BooleanQuery();

            booleanQuery.Add(
                new TermQuery(new Term(FieldDefinition.GetFieldName<ProductSearchBrandDefinition>(), brandId.ToString())),
                Occur.MUST);

            return booleanQuery;
        }
    }
}