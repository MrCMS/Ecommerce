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

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class OrderLineService : IOrderLineService
    {
        private readonly ISession _session;

        public OrderLineService(ISession session)
        {
            _session = session;
        }

        public IList<OrderLine> GetAll()
        {
            return _session.QueryOver<OrderLine>().Cacheable().List();
        }

        public void Add(OrderLine item)
        {
            _session.Transact(session => session.Save(item));
        }

        public void Save(OrderLine item)
        {
            _session.Transact(session => session.SaveOrUpdate(item));
        }

        public void Delete(OrderLine item)
        {
            _session.Transact(session => session.Delete(item));
        }
    }
}