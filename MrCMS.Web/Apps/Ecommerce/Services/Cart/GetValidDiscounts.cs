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

        public CheckCodeResult CheckCode(CartModel cart, string discountCode, bool fromUrl)
        {
            DateTime now = CurrentRequestData.Now;
            var code = discountCode.Trim();
            var query = _session.QueryOver<Discount>().Where(discount
                => (discount.Code == code && discount.RequiresCode)
                   && (discount.ValidFrom == null || discount.ValidFrom <= now)
                   && (discount.ValidUntil == null || discount.ValidUntil >= now));

            if (fromUrl)
                query = query.Where(x => x.CanBeAppliedFromUrl);

            var discounts = query.Cacheable().List();

            var checkCodeResult = new CheckCodeResult
            {
                RedirectUrl = discounts.Any()
                    ? discounts.First().RedirectUrl
                    : "/"
            };

            if (!discounts.Any())
            {
                checkCodeResult.Message = _stringResourceProvider.GetValue("The code you entered is not valid.");
                return checkCodeResult;
            }

            var checkLimitationsResults = discounts.Select(discount => _cartDiscountApplicationService.CheckLimitations(discount, cart, discounts)).ToList();
            if (checkLimitationsResults.All(result => result.Status == CheckLimitationsResultStatus.NeverValid))
            {
                checkCodeResult.Message = checkLimitationsResults.First().FormattedMessage;
                return checkCodeResult;
            }

            checkCodeResult.Success = !fromUrl || checkLimitationsResults.Any(result => result.Status != CheckLimitationsResultStatus.CurrentlyInvalid);
            checkCodeResult.Message = fromUrl && checkLimitationsResults.All(result => result.Status == CheckLimitationsResultStatus.CurrentlyInvalid)
                ? string.Format("{0} {1}", discounts.First().AppliedNotYetValidMessage, checkLimitationsResults.First().FormattedMessage)
                : discounts.First().SuccessMessage;

            return checkCodeResult;
        }
    }
}