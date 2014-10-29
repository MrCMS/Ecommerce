using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models.StockAvailability;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface ICartItemAvailablityService
    {
        CanBuyStatus CanBuy(CartItem item);
    }
}