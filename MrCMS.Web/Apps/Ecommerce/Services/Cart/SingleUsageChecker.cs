using System.Linq;
using MrCMS.Helpers;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class SingleUsageChecker : DiscountLimitationChecker<SingleUsage>
    {
        private readonly ISession _session;
        private readonly IStringResourceProvider _stringResourceProvider;

        public SingleUsageChecker(ISession session, IStringResourceProvider stringResourceProvider)
        {
            _session = session;
            _stringResourceProvider = stringResourceProvider;
        }

        public override CheckLimitationsResult CheckLimitations(SingleUsage limitation, CartModel cart)
        {
            var discountId = limitation.Discount.Id;
            var anyUsages =
                _session.QueryOver<DiscountUsage>().Where(usage => usage.Discount.Id == discountId).Cacheable().Any();

            return anyUsages
                ? CheckLimitationsResult.NeverValid(_stringResourceProvider.GetValue("This discount has already been used."))
                : CheckLimitationsResult.Successful(Enumerable.Empty<CartItem>());
        }
    }
}