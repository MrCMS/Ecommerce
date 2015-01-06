using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface ICategoryProductsAdminService
    {
        List<ProductSortData> GetProductSortData(Category category);
        bool IsSorted(Category category);
        void Update(Category category, List<ProductSortData> productSortData);
        void ClearSorting(Category category);
    }
}