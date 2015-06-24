using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class GetValidDiscounts : IGetValidDiscounts
    {
        private readonly ISession _session;
        private readonly ICartDiscountApplicationService _cartDiscountApplicationService;
        private readonly IStringResourceProvider _stringResourceProvider;

        public GetValidDiscounts(ISession session, ICartDiscountApplicationService cartDiscountApplicationService, IStringResourceProvider stringResourceProvider)
        {
            _session = session;
            _cartDiscountApplicationService = cartDiscountApplicationService;
            _stringResourceProvider = stringResourceProvider;
        }

        public IList<Discount> Get(CartModel cart, List<string> discountCodes)
        {
            DateTime now = CurrentRequestData.Now;
            return _session.QueryOver<Discount>()
                 .Where(
                     discount =>
                         (discount.Code.IsIn(discountCodes) || !discount.RequiresCode) &&
                         (discount.ValidFrom == null || discount.ValidFrom <= now) &&
                         (discount.ValidUntil == null || discount.ValidUntil >= now))
                 .Cacheable().List();
        }

        public CheckCodeResult CheckCode(CartModel cart, string discountCode)
        {
            DateTime now = CurrentRequestData.Now;
            var code = discountCode.Trim();
            var discounts = _session.QueryOver<Discount>()
                .Where(
                    discount =>
                        (discount.Code == code && discount.RequiresCode) &&
                        (discount.ValidFrom == null || discount.ValidFrom <= now) &&
                        (discount.ValidUntil == null || discount.ValidUntil >= now))
                .Cacheable().List();

            if (!discounts.Any())
            {
                return new CheckCodeResult
                {
                    Message = _stringResourceProvider.GetValue("The code you entered is not valid.")
                };
            }

            var checkLimitationsResults = discounts.Select(
                discount =>
                    _cartDiscountApplicationService.CheckLimitations(discount, cart, discounts)).ToList();
            if (checkLimitationsResults.All(result => result.Status == CheckLimitationsResultStatus.NeverValid))
            {
                return new CheckCodeResult
                {
                    Message = checkLimitationsResults.First().FormattedMessage
                };
            }
            return new CheckCodeResult { Success = true };
        }
    }
}