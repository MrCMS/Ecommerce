using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface IGetValidDiscounts
    {
        IList<Discount> Get(CartModel cart, List<string> discountCodes);
    }
}