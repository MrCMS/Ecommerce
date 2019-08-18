using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Multisite;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class CartDiscountService : ICartDiscountService
    {
        private readonly CartModel _cart;
        private readonly Site _site;
        private readonly ICartDiscountCodeService _cartDiscountCodeService;
        private readonly IGetValidDiscounts _getValidDiscounts;

        public CartDiscountService(ICartDiscountCodeService cartDiscountCodeService,
            IGetValidDiscounts getValidDiscounts, CartModel cart, Site site)
        {
            _cartDiscountCodeService = cartDiscountCodeService;
            _getValidDiscounts = getValidDiscounts;
            _cart = cart;
            _site = site;
        }

        public CheckCodeResult AddDiscountCode(string code, bool fromUrl, Uri referrer = null)
        {
            HashSet<string> codes = _cartDiscountCodeService.Get();
            CheckCodeResult codeResult = _getValidDiscounts.CheckCode(_cart, code, fromUrl);
            if (codeResult.Success)
            {
                codes.Add(code);
                _cartDiscountCodeService.SaveDiscounts(codes);
            }
            if (referrer != null && (!referrer.IsAbsoluteUri || referrer.Authority == _site.BaseUrl))
            {
                codeResult.RedirectUrl = referrer.ToString();
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