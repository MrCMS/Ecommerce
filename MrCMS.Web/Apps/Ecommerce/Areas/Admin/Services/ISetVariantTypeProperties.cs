using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface ISetVariantTypeProperties
    {
        void SetProperties(ProductVariant productVariant, string variantType);
    }
}