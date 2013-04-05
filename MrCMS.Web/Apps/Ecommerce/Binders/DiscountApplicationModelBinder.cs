using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Services;
using NHibernate;
using NHibernate.Criterion;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website.Binders;

namespace MrCMS.Web.Apps.Ecommerce.Binders
{
    public abstract class DiscountApplicationModelBinder : MrCMSDefaultModelBinder
    {
        private readonly IDiscountApplicationService _discountApplicationService;

        protected DiscountApplicationModelBinder(ISession session, IDiscountApplicationService discountApplicationService)
            : base(() => session)
        {
            this._discountApplicationService = discountApplicationService;
        }
    }
}