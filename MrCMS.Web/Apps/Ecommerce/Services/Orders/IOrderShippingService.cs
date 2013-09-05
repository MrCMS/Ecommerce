using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public interface IOrderShippingService
    {
        Dictionary<string, List<string>> BulkShippingUpdate(Stream file);

        List<SelectListItem> GetShippingOptions(CartModel cart);
        List<SelectListItem> GetCheapestShippingOptions(CartModel cart);
        ShippingMethod GetDefaultShippingMethod(CartModel cart);
        IEnumerable<ShippingCalculation> GetCheapestShippingCalculationsForEveryCountry(CartModel cart);
    }
}