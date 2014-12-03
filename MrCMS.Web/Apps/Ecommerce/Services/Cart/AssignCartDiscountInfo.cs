using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class AssignCartDiscountInfo : IAssignCartDiscountInfo
    {
        private readonly ICartDiscountApplicationService _cartDiscountApplicationService;
        private readonly ICartSessionManager _cartSessionManager;
        private readonly ISession _session;

        public AssignCartDiscountInfo(ISession session, ICartSessionManager cartSessionManager,
            ICartDiscountApplicationService cartDiscountApplicationService)
        {
            _session = session;
            _cartSessionManager = cartSessionManager;
            _cartDiscountApplicationService = cartDiscountApplicationService;
        }

        public CartModel Assign(CartModel cart, Guid userGuid)
        {
            cart.DiscountCodes = GetDiscountCodes(userGuid);
            cart.Discounts = GetDiscounts(userGuid, cart);
            cart.SetDiscountApplication(GetDiscountApplication(cart.Discounts, cart));
            return cart;
        }

        private DiscountApplicationInfo GetDiscountApplication(List<DiscountInfo> discounts, CartModel cart)
        {
            var discountApplicationInfo = new DiscountApplicationInfo();
            foreach (var discountInfo in discounts.FindAll(x => x.Status == DiscountStatus.ToApply))
            {
                discountApplicationInfo.Add(_cartDiscountApplicationService.ApplyDiscount(discountInfo, cart));
            }
            return discountApplicationInfo;
        }

        private List<DiscountInfo> GetDiscounts(Guid userGuid, CartModel cart)
        {
            List<string> discountCodes = GetDiscountCodes(userGuid);
            DateTime now = CurrentRequestData.Now;
            IList<Discount> discounts = _session.QueryOver<Discount>()
                .Where(
                    discount =>
                        (discount.Code.IsIn(discountCodes) || !discount.RequiresCode) &&
                        (discount.ValidFrom == null || discount.ValidFrom <= now) &&
                        (discount.ValidUntil == null || discount.ValidUntil >= now))
                .Cacheable().List();

            var discountInfos = (from discount in discounts
                                 let result = _cartDiscountApplicationService.CheckLimitations(discount, cart)
                                 select new DiscountInfo(discount, result)).Where(x => x.Status != DiscountStatus.AutomaticAndInvalid)
                .ToList();

            return discountInfos;
        }

        private List<string> GetDiscountCodes(Guid userGuid)
        {
            return _cartSessionManager.GetSessionValue(CartDiscountService.CurrentDiscountCodesKey,
                userGuid, new List<string>());
        }
    }
}