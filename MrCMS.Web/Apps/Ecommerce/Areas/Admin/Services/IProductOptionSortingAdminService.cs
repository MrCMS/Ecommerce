using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IProductOptionSortingAdminService
    {
        List<ProductOptionSortingData> Search(ProductOptionSortingSearchQuery searchQuery);
        List<ProductOptionValueSortingData> GetOptions(ProductOption productOption);
        void SaveSorting(ProductOption option, List<ProductOptionValueSortingData> sortingInfo);
    }
}