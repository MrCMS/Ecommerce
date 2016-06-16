using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class AssignCartDiscountInfo : IAssignCartDiscountInfo
    {
        private readonly ICartDiscountApplicationService _cartDiscountApplicationService;
        private readonly ICartDiscountCodeService _cartDiscountCodeService;
        private readonly IGetValidDiscounts _getValidDiscounts;

        public AssignCartDiscountInfo(ICartDiscountApplicationService cartDiscountApplicationService,
            ICartDiscountCodeService cartDiscountCodeService, IGetValidDiscounts getValidDiscounts)
        {
            _cartDiscountApplicationService = cartDiscountApplicationService;
            _cartDiscountCodeService = cartDiscountCodeService;
            _getValidDiscounts = getValidDiscounts;
        }

        public CartModel Assign(CartModel cart, Guid userGuid)
        {
            cart.DiscountCodes = GetDiscountCodes(userGuid);
            var discounts = GetDiscounts(userGuid, cart);
            var result = GetDiscountApplication(discounts, cart);
            cart.SetDiscountApplication(result.Info);
            foreach (var info in discounts.Where(info => result.AppliedDiscounts.Contains(info.Discount)))
            {
                info.MarkAsApplied();
            }
            cart.Discounts = discounts;
            return cart;
        }

        private DiscountApplicationResult GetDiscountApplication(List<DiscountInfo> discounts, CartModel cart)
        {
            var discountApplicationInfo = new DiscountApplicationInfo();
            var hashSet = new List<Discount>();
            foreach (var discountInfo in discounts.FindAll(x => x.Status == DiscountStatus.ValidButNotApplied))
            {
                var applicationInfo = _cartDiscountApplicationService.ApplyDiscount(discountInfo, cart);
                if (!applicationInfo.IsApplied)
                    continue;

                discountApplicationInfo.Add(applicationInfo);
                hashSet.Add(discountInfo.Discount);
            }
            return new DiscountApplicationResult
            {
                Info = discountApplicationInfo,
                AppliedDiscounts = hashSet
            };
        }

        private List<DiscountInfo> GetDiscounts(Guid userGuid, CartModel cart)
        {
            List<string> discountCodes = GetDiscountCodes(userGuid);
            IList<Discount> discounts = _getValidDiscounts.Get(cart, discountCodes);

            var discountInfos = (from discount in discounts
                                 let result = _cartDiscountApplicationService.CheckLimitations(discount, cart, discounts)
                                 select new DiscountInfo(discount, result)).Where(
                    info =>
                        info.Status != DiscountStatus.AutomaticAndInvalid && info.Status != DiscountStatus.NeverValid)
                .ToList();

            return discountInfos;
        }

        private List<string> GetDiscountCodes(Guid userGuid)
        {
            return _cartDiscountCodeService.Get(userGuid).ToList();
        }
    }
}