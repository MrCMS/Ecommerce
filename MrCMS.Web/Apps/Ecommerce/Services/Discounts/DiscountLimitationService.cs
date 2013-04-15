using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Discounts
{
    public class DiscountLimitationService : IDiscountLimitationService
    {
        private readonly ISession _session;

        public DiscountLimitationService(ISession session)
        {
            _session = session;
        }

        public DiscountLimitation Get(int id)
        {
            return _session.QueryOver<DiscountLimitation>().Where(x => x.Id == id).Cacheable().SingleOrDefault();
        }
    }
}