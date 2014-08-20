using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IProductVariantAdminService
    {
        void SetViewData(ViewDataDictionary viewData, ProductVariant productVariant);
        void Add(ProductVariant productVariant);
        void Delete(ProductVariant productVariant);
        bool AnyExistingProductVariantWithSKU(string sku, int id);
        void Update(ProductVariant productVariant);
    }
}