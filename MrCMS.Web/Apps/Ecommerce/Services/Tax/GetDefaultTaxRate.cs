using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Tax
{
    public class GetDefaultTaxRate : IGetDefaultTaxRate
    {
        private readonly ISession _session;

        public GetDefaultTaxRate(ISession session)
        {
            _session = session;
        }

        public TaxRate Get()
        {
            return _session.QueryOver<TaxRate>().Where(x => x.IsDefault).Cacheable().SingleOrDefault();
        }
    }
}