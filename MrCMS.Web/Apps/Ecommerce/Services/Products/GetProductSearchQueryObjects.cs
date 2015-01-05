using Lucene.Net.Documents;
using Lucene.Net.Search;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Indexing;
using MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class GetProductSearchQueryObjects : IGetProductSearchQueryObjects
    {
        private readonly IGetProductSearchSortByValue _getProductSearchSortByValue;
        private readonly IGetProductSearchLuceneQuery _getProductSearchLuceneQuery;

        public GetProductSearchQueryObjects(IGetProductSearchSortByValue getProductSearchSortByValue, IGetProductSearchLuceneQuery getProductSearchLuceneQuery)
        {
            _getProductSearchSortByValue = getProductSearchSortByValue;
            _getProductSearchLuceneQuery = getProductSearchLuceneQuery;
        }

        public Query GetQuery(ProductSearchQuery searchQuery)
        {
            return _getProductSearchLuceneQuery.Get(searchQuery);
           
        }

        public Filter GetFilter(ProductSearchQuery query)
        {
            string dateValue = DateTools.DateToString(CurrentRequestData.Now, DateTools.Resolution.SECOND);
            FieldCacheRangeFilter<string> filter =
                FieldCacheRangeFilter.NewStringRange(FieldDefinition.GetFieldName<ProductSearchPublishOnDefinition>(),
                    null,
                    dateValue, false, true);
            return filter;
        }

        public Sort GetSort(ProductSearchQuery query)
        {
            switch (_getProductSearchSortByValue.Get(query))
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
                    return
                        new Sort(new[]
                        {
                            new SortField(FieldDefinition.GetFieldName<ProductSearchNameSortDefinition>(),
                                SortField.STRING)
                        });
                case ProductSearchSort.NameZToA:
                    return
                        new Sort(new[]
                        {
                            new SortField(FieldDefinition.GetFieldName<ProductSearchNameSortDefinition>(),
                                SortField.STRING, true)
                        });
                case ProductSearchSort.PriceLowToHigh:
                    return
                        new Sort(new[] { new SortField(FieldDefinition.GetFieldName<ProductSearchPriceDefinition>(), SortField.DOUBLE) });
                case ProductSearchSort.PriceHighToLow:
                    return
                        new Sort(new[]
                        {
                            new SortField(FieldDefinition.GetFieldName<ProductSearchPriceDefinition>(), SortField.DOUBLE,
                                true)
                        });
                case ProductSearchSort.DisplayOrder:
                    if (query.CategoryId != null)
                        return
                            new Sort(new[]
                            {
                                new SortField(ProductSearchIndex.GetCategoryFieldName(query.CategoryId.Value), SortField.INT,
                                    false)
                            });
                    return Sort.RELEVANCE;
                default:
                    return Sort.RELEVANCE;
            }
        }
    }
}