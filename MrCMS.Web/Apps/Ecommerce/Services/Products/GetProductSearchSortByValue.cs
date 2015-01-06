using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Settings;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class GetProductSearchSortByValue : IGetProductSearchSortByValue
    {
        private readonly ISession _session;
        private readonly EcommerceSettings _ecommerceSettings;

        public GetProductSearchSortByValue(ISession session,EcommerceSettings ecommerceSettings)
        {
            _session = session;
            _ecommerceSettings = ecommerceSettings;
        }

        public ProductSearchSort Get(ProductSearchQuery query)
        {
            if (query.SortBy.HasValue)
                return query.SortBy.Value;
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
                return ProductSearchSort.Relevance;
            if (query.CategoryId.HasValue)
            {
                var category = _session.Get<Category>(query.CategoryId);
                if (category != null)
                    if (category.DefaultProductSearchSort.HasValue)
                        return category.DefaultProductSearchSort.Value;
                    else if (CategoryHasBeenSorted(query))
                        return ProductSearchSort.DisplayOrder;
            }
            return _ecommerceSettings.DefaultProductSearchSort;
        }

        public  bool CategoryHasBeenSorted(ProductSearchQuery query)
        {
            if (!query.CategoryId.HasValue)
                return false;
            return
                _session.QueryOver<CategoryProductDisplayOrder>()
                    .Where(order => order.Category.Id == query.CategoryId.Value)
                    .Cacheable()
                    .Any();
        }
    }
}