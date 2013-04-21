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
    public class OrderRefundService : IOrderRefundService
    {
        private readonly ISession _session;

        public OrderRefundService(ISession session)
        {
            _session = session;
        }

        public IList<OrderRefund> GetAll()
        {
            return _session.QueryOver<OrderRefund>().Cacheable().List();
        }

        public void Save(OrderRefund item)
        {
            _session.Transact(session => session.SaveOrUpdate(item));
        }

        public void Delete(OrderRefund item)
        {
            _session.Transact(session => session.Delete(item));
        }
    }
}