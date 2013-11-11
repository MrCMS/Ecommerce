using MrCMS.Web.Apps.Ecommerce.Models;
using SagePayMvc;

namespace MrCMS.Web.Apps.Ecommerce.Services.SagePay
{
    public interface ISagePayItemCreator
    {
        ShoppingBasket GetShoppingBasket(CartModel model);
        Address GetAddress(Entities.Users.Address address);
    }
}