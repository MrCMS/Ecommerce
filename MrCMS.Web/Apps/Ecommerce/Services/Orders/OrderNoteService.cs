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
    public class OrderNoteService : IOrderNoteService
    {
        private readonly ISession _session;

        public OrderNoteService(ISession session)
        {
            _session = session;
        }

        public IList<OrderNote> GetAll()
        {
            return _session.QueryOver<OrderNote>().Cacheable().List();
        }

        public void Add(OrderNote item)
        {
            _session.Transact(session => session.Save(item));
        }

        public void Save(OrderNote item)
        {
            _session.Transact(session => session.SaveOrUpdate(item));
        }

        public void Delete(OrderNote item)
        {
            _session.Transact(session => session.Delete(item));
        }
    }
}