using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IProductVariantService
    {
        IList<ProductVariant> GetAll();
        ProductVariant GetProductVariantBySKU(string sku);
        void Add(ProductVariant productVariant);
        void Update(ProductVariant productVariant);
        void Delete(ProductVariant productVariant);
        bool AnyExistingProductVariantWithSKU(string sku, int id);
        ProductVariant Get(int id);
        List<SelectListItem> GetOptions();
        PriceBreak AddPriceBreak(AddPriceBreakModel model);
        bool IsPriceBreakQuantityValid(int quantity, ProductVariant productVariant);
        bool IsPriceBreakPriceValid(decimal price, ProductVariant productVariant, int quantity);
        void DeletePriceBreak(PriceBreak priceBreak);
    }
}