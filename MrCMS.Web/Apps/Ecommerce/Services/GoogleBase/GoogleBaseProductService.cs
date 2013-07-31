using NHibernate;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.GoogleBase;

namespace MrCMS.Web.Apps.Ecommerce.Services.GoogleBase
{
    public class GoogleBaseProductService : IGoogleBaseProductService
    {
        private readonly ISession _session;

        public GoogleBaseProductService(ISession session)
        {
            _session = session;
        }

        public GoogleBaseProduct Get(int id)
        {
            return _session.QueryOver<GoogleBaseProduct>().Where(x=>x.Id==id).SingleOrDefault();
        }

        public void Add(GoogleBaseProduct item)
        {
            _session.Transact(session => session.Save(item));
        }

        public void Update(GoogleBaseProduct item)
        {
            _session.Transact(session => session.Update(item));
        }
    }
}