using System;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class AssignCartDiscountInfo : IAssignCartDiscountInfo
    {
        private readonly ICartSessionManager _cartSessionManager;
        private readonly ISession _session;

        public AssignCartDiscountInfo(ISession session, ICartSessionManager cartSessionManager)
        {
            _session = session;
            _cartSessionManager = cartSessionManager;
        }

        public CartModel Assign(CartModel cart, Guid userGuid)
        {
            cart.DiscountCode = GetDiscountCode(userGuid);
            cart.Discount = GetDiscount(userGuid);
            cart.Items.ForEach(item => item.SetDiscountInfo(cart));
            return cart;
        }

        private Discount GetDiscount(Guid userGuid)
        {
            return !String.IsNullOrWhiteSpace(GetDiscountCode(userGuid))
                ? _session.QueryOver<Discount>()
                    .Where(discount => discount.Code.IsInsensitiveLike(GetDiscountCode(userGuid), MatchMode.Exact))
                    .Cacheable()
                    .SingleOrDefault()
                : null;
        }

        private string GetDiscountCode(Guid userGuid)
        {
            return _cartSessionManager.GetSessionValue<string>(CartDiscountService.CurrentDiscountCodeKey, userGuid);
        }
    }
}