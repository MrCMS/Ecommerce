using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Discounts
{
    public class DiscountApplicationService : IDiscountApplicationService
    {
        private readonly ISession _session;

        public DiscountApplicationService(ISession session)
        {
            _session = session;
        }

        public DiscountApplication Get(int id)
        {
            return _session.QueryOver<DiscountApplication>().Where(x => x.Id == id).Cacheable().SingleOrDefault();
        }
    }
}