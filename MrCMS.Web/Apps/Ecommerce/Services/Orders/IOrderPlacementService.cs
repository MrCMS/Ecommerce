using System;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public interface IOrderPlacementService
    {
        Order PlaceOrder(CartModel cartModel, Action<Order> postCreationActions);
    }
}