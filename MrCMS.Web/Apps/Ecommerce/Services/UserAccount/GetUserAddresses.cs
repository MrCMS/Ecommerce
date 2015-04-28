using System.Collections.Generic;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.UserAccount
{
    public class GetUserAddresses : IGetUserAddresses
    {
        private readonly ISession _session;

        public GetUserAddresses(ISession session)
        {
            _session = session;
        }

        public IList<Address> Get(User user)
        {
            return _session.QueryOver<Address>().OrderBy(x => x.Id).Desc.Cacheable().List();
        }
    }
}