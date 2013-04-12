using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Discounts;
using NHibernate;
using NHibernate.Criterion;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website.Binders;

namespace MrCMS.Web.Apps.Ecommerce.Binders
{
    public abstract class DiscountLimitationModelBinder : MrCMSDefaultModelBinder
    {
        private readonly IDiscountLimitationService _discountLimitationService;

        protected DiscountLimitationModelBinder(ISession session, IDiscountLimitationService discountLimitationService)
            : base(() => session)
        {
            this._discountLimitationService = discountLimitationService;
        }
    }
}