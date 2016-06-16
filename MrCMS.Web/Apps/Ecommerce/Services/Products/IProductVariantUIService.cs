using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models.StockAvailability;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IProductVariantUIService
    {
        CanBuyStatus CanBuyAny(ProductVariant productVariant);
        List<SelectListItem> GetProductVariantOptions(ProductVariant productVariant, bool showName = true, bool showOptionValues = true);
    }
}