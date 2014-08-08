using System;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface IGetShippingAddress
    {
        Address Get(Guid userGuid);
    }
}