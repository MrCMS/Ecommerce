using System.Collections.Generic;
using System.Linq;
using Microsoft.Ajax.Utilities;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using NHibernate;
using NHibernate.Transform;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class ProductOptionSortingAdminService : IProductOptionSortingAdminService
    {
        private readonly ISession _session;

        public ProductOptionSortingAdminService(ISession session)
        {
            _session = session;
        }

        public List<ProductOptionSortingData> Search(ProductOptionSortingSearchQuery searchQuery)
        {
            ProductOption productOptionAlias = null;
            IQueryOver<ProductOption, ProductOption> queryOver = _session.QueryOver(() => productOptionAlias);

            ProductOptionSortingData sortingDataAlias = null;
            queryOver.SelectList(builder =>
                builder
                    .Select(() => productOptionAlias.Id).WithAlias(() => sortingDataAlias.Id)
                    .Select(() => productOptionAlias.Name).WithAlias(() => sortingDataAlias.Name)
                );

            return queryOver.TransformUsing(Transformers.AliasToBean<ProductOptionSortingData>())
                .List<ProductOptionSortingData>().ToList();
        }

        public List<ProductOptionValueSortingData> GetOptions(ProductOption productOption)
        {
            ProductOption productOptionAlias = null;
            List<ProductOptionValue> productOptionValues = _session.QueryOver<ProductOptionValue>()
                .JoinAlias(value => value.ProductOption, () => productOptionAlias)
                .Where(() => productOptionAlias.Id == productOption.Id)
                .Cacheable().List().DistinctBy(value => value.Value).ToList();

            IList<ProductOptionValueSort> productOptionValueSorts = GetProductOptionValueSorts(productOption);

            return productOptionValues
                .Select(value =>
                {
                    ProductOptionValueSort order =
                        productOptionValueSorts.FirstOrDefault(sort => sort.Value == value.Value);
                    return new ProductOptionValueSortingData
                    {
                        Value = value.Value,
                        DisplayOrder = order == null ? productOptionValues.Count() : order.DisplayOrder
                    };
                }).OrderBy(data => data.DisplayOrder).ThenBy(data => data.Value).ToList();
        }

        public void SaveSorting(ProductOption option, List<ProductOptionValueSortingData> sortingInfo)
        {
            IList<ProductOptionValueSort> productOptionValueSorts = GetProductOptionValueSorts(option);

            _session.Transact(session =>
            {
                foreach (ProductOptionValueSortingData data in sortingInfo)
                {
                    ProductOptionValueSort existingSort =
                        productOptionValueSorts.FirstOrDefault(sort => sort.Value == data.Value);
                    if (existingSort != null)
                    {
                        existingSort.DisplayOrder = data.DisplayOrder;
                        session.Update(existingSort);
                        continue;
                    }
                    existingSort = new ProductOptionValueSort
                    {
                        ProductOption = option,
                        DisplayOrder = data.DisplayOrder,
                        Value = data.Value
                    };
                    session.Save(existingSort);
                }
            });
        }

        private IList<ProductOptionValueSort> GetProductOptionValueSorts(ProductOption productOption)
        {
            return _session.QueryOver<ProductOptionValueSort>()
                .Where(sort => sort.ProductOption.Id == productOption.Id)
                .Cacheable().List();
        }
    }
}