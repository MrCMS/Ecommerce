using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IEcommerceSearchablePageAdminService
    {
        List<ProductSpecificationAttribute> GetShownSpecifications(EcommerceSearchablePage category);
        bool AddSpecificationToHidden(ProductSpecificationAttribute attribute, int categoryId);
        bool RemoveSpecificationFromHidden(ProductSpecificationAttribute attribute, int categoryId);
    }
}