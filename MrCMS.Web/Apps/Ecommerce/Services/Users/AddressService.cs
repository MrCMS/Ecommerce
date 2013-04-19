using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using NHibernate;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;

namespace MrCMS.Web.Apps.Ecommerce.Services.Users
{
    public class AddressService : IAddressService
    {
        private readonly ISession _session;

        public AddressService(ISession session)
        {
            _session = session;
        }

        public Address Get(int id)
        {
            return _session.QueryOver<Address>().Where(x => x.Id == id).Cacheable().SingleOrDefault();
        }

        public void Save(Address item)
        {
            _session.Transact(session => session.SaveOrUpdate(item));
        }
    }
}