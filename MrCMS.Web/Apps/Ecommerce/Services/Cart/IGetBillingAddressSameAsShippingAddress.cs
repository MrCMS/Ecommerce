using System;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface IGetBillingAddressSameAsShippingAddress
    {
        bool Get(CartModel cart, Guid userGuid);
    }
}