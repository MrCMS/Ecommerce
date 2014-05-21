using System.Collections.Generic;
using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface ICategoryAdminService
    {
        CategoryPagedList Search(string searchTerm, int page = 1);
        IEnumerable<AutoCompleteResult> Search(string query, List<int> ids);
        bool CategoryContainerExists();
        List<ProductSpecificationAttribute> GetShownSpecifications(Category category);
        bool AddSpecificationToHidden(ProductSpecificationAttribute attribute, int categoryId);
        bool RemoveSpecificationFromHidden(ProductSpecificationAttribute attribute, int categoryId);
    }
}