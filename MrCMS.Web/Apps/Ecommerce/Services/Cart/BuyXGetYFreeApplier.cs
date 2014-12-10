using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class BuyXGetYFreeApplier : CartItemBasedDiscountApplicationApplier<BuyXGetYFree>
    {
        public BuyXGetYFreeApplier(IGetCartItemBasedApplicationProducts getCartItemBasedApplicationProducts)
            : base(getCartItemBasedApplicationProducts)
        {
        }

        public override DiscountApplicationInfo Apply(BuyXGetYFree application, CartModel cart,
            CheckLimitationsResult checkLimitationsResult)
        {
            IEnumerable<CartItem> cartItems = GetItems(application, checkLimitationsResult, cart);
            var prices = new List<ItemPrice>();
            foreach (CartItem cartItem in cartItems)
            {
                for (int i = 0; i < cartItem.Quantity; i++)
                {
                    prices.Add(new ItemPrice { Id = cartItem.Id, UnitPrice = cartItem.UnitPrice });
                }
            }
            var numberFree = (prices.Count/(application.BuyAmount + application.FreeAmount))*application.FreeAmount;
            var itemPrices = prices.OrderBy(x => x.UnitPrice).Take(numberFree);

            return new DiscountApplicationInfo
            {
                ItemDiscounts = itemPrices.GroupBy(x => x.Id).ToDictionary(cartItem => cartItem.Key, item => item.Sum(x => x.UnitPrice))
            };
        }
    }
}