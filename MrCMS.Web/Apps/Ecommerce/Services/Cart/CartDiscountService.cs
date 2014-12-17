using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class CartDiscountService : ICartDiscountService
    {
        private readonly ICartDiscountCodeService _cartDiscountCodeService;
        private readonly IGetValidDiscounts _getValidDiscounts;
        private readonly CartModel _cart;

        public CartDiscountService(ICartDiscountCodeService cartDiscountCodeService, IGetValidDiscounts getValidDiscounts, CartModel cart)
        {
            _cartDiscountCodeService = cartDiscountCodeService;
            _getValidDiscounts = getValidDiscounts;
            _cart = cart;
        }


        public bool AddDiscountCode(string code)
        {
            var codes = _cartDiscountCodeService.Get();
            var discounts = _getValidDiscounts.Get(_cart, new List<string> { code });
            if (!discounts.Any())
                return false;
            var result = codes.Add(code);
            _cartDiscountCodeService.SaveDiscounts(codes);
            return result;
        }

        public void RemoveDiscountCode(string discountCode)
        {
            var codes = _cartDiscountCodeService.Get();
            codes.Remove(discountCode);
            _cartDiscountCodeService.SaveDiscounts(codes);
        }
    }
}