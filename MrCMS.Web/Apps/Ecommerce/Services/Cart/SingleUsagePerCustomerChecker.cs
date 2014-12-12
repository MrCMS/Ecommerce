using System.Linq;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class SingleUsagePerCustomerChecker : DiscountLimitationChecker<SingleUsagePerCustomer>
    {
        private readonly ISession _session;
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly IGetCurrentUser _getCurrentUser;

        public SingleUsagePerCustomerChecker(ISession session, IStringResourceProvider stringResourceProvider, IGetCurrentUser getCurrentUser)
        {
            _session = session;
            _stringResourceProvider = stringResourceProvider;
            _getCurrentUser = getCurrentUser;
        }

        public override CheckLimitationsResult CheckLimitations(SingleUsagePerCustomer limitation, CartModel cart)
        {
            var user = _getCurrentUser.Get();
            if (user == null)
                return CheckLimitationsResult.NeverValid(_stringResourceProvider.GetValue("This discount requires an account."));

            var discountId = limitation.Discount.Id;
            var anyUsages =
                _session.Query<DiscountUsage>()
                    .Where(usage => usage.Discount.Id == discountId && usage.Order.User.Id == user.Id)
                    .Cacheable()
                    .Any();

            return anyUsages
                ? CheckLimitationsResult.NeverValid(_stringResourceProvider.GetValue("This discount has already been used."))
                : CheckLimitationsResult.Successful(Enumerable.Empty<CartItem>());
        }
    }
}