using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Models.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IGetCheapestShippingMethod
    {
        ShippingAmount Get(CartModel cart);
    }
}