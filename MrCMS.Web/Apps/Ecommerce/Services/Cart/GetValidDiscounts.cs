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
            var query = _session.QueryOver<Discount>().Where(x
                => (x.Code == code && x.RequiresCode)
                   && (x.ValidFrom == null || x.ValidFrom <= now)
                   && (x.ValidUntil == null || x.ValidUntil >= now));

            if (fromUrl)
                query = query.Where(x => x.CanBeAppliedFromUrl);

            var discount = query.Cacheable().List().FirstOrDefault();

            var defaultRedirectUrl = "/";
            var checkCodeResult = new CheckCodeResult
            {
                RedirectUrl = discount != null
                    ? string.IsNullOrEmpty(discount.RedirectUrl)
                        ? defaultRedirectUrl
                        : discount.RedirectUrl
                    : defaultRedirectUrl,
            };

            if (discount == null)
            {
                checkCodeResult.Message = _stringResourceProvider.GetValue("The code you entered is not valid.");
                return checkCodeResult;
            }
            var discounts = Get(cart,cart.DiscountCodes);
            discounts.Add(discount);

            var checkLimitationsResult = _cartDiscountApplicationService.CheckLimitations(discount, cart, discounts);
            if (checkLimitationsResult.Status == CheckLimitationsResultStatus.NeverValid)
            {
                checkCodeResult.Message = checkLimitationsResult.FormattedMessage;
                return checkCodeResult;
            }

            checkCodeResult.Status =
                checkLimitationsResult.Status == CheckLimitationsResultStatus.CurrentlyInvalid
                    ? CheckLimitationsResultStatus.CurrentlyInvalid
                    : CheckLimitationsResultStatus.Success;
            if (fromUrl && checkLimitationsResult.Status == CheckLimitationsResultStatus.CurrentlyInvalid)
            {
                checkCodeResult.Message = string.IsNullOrWhiteSpace(discount.AppliedNotYetValidMessage)
                    ? checkLimitationsResult.FormattedMessage
                    : discount.AppliedNotYetValidMessage;
            }
            else
            {
                checkCodeResult.Message = discount.SuccessMessage;
            }

            return checkCodeResult;
        }
    }
}