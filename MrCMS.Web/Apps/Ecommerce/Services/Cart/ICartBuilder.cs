using System;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface ICartBuilder
    {
        CartModel BuildCart();
        CartModel BuildCart(Guid userGuid);
    }
}