using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class CartDiscountService : ICartDiscountService
    {
        private readonly CartModel _cart;
        private readonly ICartDiscountCodeService _cartDiscountCodeService;
        private readonly IGetValidDiscounts _getValidDiscounts;

        public CartDiscountService(ICartDiscountCodeService cartDiscountCodeService,
            IGetValidDiscounts getValidDiscounts, CartModel cart)
        {
            _cartDiscountCodeService = cartDiscountCodeService;
            _getValidDiscounts = getValidDiscounts;
            _cart = cart;
        }

        public CheckCodeResult AddDiscountCode(string code, bool fromUrl)
        {
            HashSet<string> codes = _cartDiscountCodeService.Get();
            CheckCodeResult codeResult = _getValidDiscounts.CheckCode(_cart, code, fromUrl);
            if (codeResult.Success)
            {
                codes.Add(code);
                _cartDiscountCodeService.SaveDiscounts(codes);
            }
            return codeResult;
        }

        public void RemoveDiscountCode(string discountCode)
        {
            HashSet<string> codes = _cartDiscountCodeService.Get();
            var matchedCode = codes.FirstOrDefault(x => x.Equals(discountCode, StringComparison.InvariantCultureIgnoreCase));
            if (matchedCode != null)
                codes.Remove(matchedCode);
            _cartDiscountCodeService.SaveDiscounts(codes);
        }
    }
}