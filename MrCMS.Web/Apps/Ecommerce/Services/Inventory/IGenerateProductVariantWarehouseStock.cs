using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory
{
    public interface IGenerateProductVariantWarehouseStock
    {
        void Generate(ProductVariant productVariant);
    }
}