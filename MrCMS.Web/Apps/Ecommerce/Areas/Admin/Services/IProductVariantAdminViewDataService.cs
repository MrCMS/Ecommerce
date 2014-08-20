using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IProductVariantAdminViewDataService
    {
        void SetViewData(ViewDataDictionary viewData, ProductVariant productVariant);
    }
}