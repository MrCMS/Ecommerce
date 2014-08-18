using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class AssignGiftCardInfo : IAssignGiftCardInfo
    {
        private readonly ICartSessionManager _cartSessionManager;
        private readonly ISession _session;

        public AssignGiftCardInfo(ICartSessionManager cartSessionManager,ISession session)
        {
            _cartSessionManager = cartSessionManager;
            _session = session;
        }

        public CartModel Assign(CartModel cart, Guid userGuid)
        {
            List<string> codes = _cartSessionManager.GetSessionValue(CartManager.CurrentAppliedGiftCards, userGuid,
                new List<string>());

            if (codes.Any())
            {
                var giftCards = _session.QueryOver<GiftCard>().Where(card => card.Code.IsIn(codes)).Cacheable().List();

                cart.AppliedGiftCards.AddRange(giftCards.Where(card => card.IsValidToUse()));
            }
            return cart;
        }
    }
}