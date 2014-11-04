using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IProductShippingChecker
    {
        bool CanShip(ProductVariant productVariant);
    }
}