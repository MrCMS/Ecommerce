using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.UserAccount
{
    public class ExistingAddressService : IExistingAddressService
    {
        private readonly ISession _session;

        public ExistingAddressService(ISession session)
        {
            _session = session;
        }

        public void Update(Address address)
        {
            _session.Transact(session => session.Update(address));
        }

        public void Delete(Address address)
        {
            _session.Transact(session => session.Delete(address));
        }
    }
}