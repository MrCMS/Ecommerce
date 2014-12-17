using System;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface ICartDiscountCodeService
    {
        HashSet<string> Get(Guid? guid = null);
        void SaveDiscounts(HashSet<string> codes);
    }
}