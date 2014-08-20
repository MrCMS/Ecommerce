using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.GoogleBase
{
    public interface IGoogleBaseShippingService
    {
        IEnumerable<GoogleBaseCalculationInfo> GetCheapestCalculationsForEachCountry(CartModel cart);
    }
}