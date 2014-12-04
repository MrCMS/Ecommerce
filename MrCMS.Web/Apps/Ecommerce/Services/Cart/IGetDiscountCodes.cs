using System;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface IGetDiscountCodes
    {
        List<string> Get(Guid userGuid);
    }
}