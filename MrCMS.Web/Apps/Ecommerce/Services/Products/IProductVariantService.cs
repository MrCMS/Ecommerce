using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IProductVariantService
    {
        ProductVariant GetProductVariantBySKU(string sku);
        void Add(ProductVariant productVariant);
        void Update(ProductVariant productVariant);
        void Delete(ProductVariant productVariant);
        bool AnyExistingProductVariantWithSKU(string sku, int id);
        IList<PriceBreak> GetAllPriceBreaksForProductVariant(int id);
        ProductVariant Get(int id);
    }
}