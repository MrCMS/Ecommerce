using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using NHibernate;
namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class DiscountManager : IDiscountManager
    {
        private readonly ISession _session;

        public DiscountManager(ISession session)
        {
            _session = session;
        }

        public IList<Discount> GetAll()
        {
            return _session.QueryOver<Discount>().CacheMode(CacheMode.Refresh).List();
        }

        public Discount Get(int DiscountId)
        {
            return _session.QueryOver<Discount>().Where(x => x.Id == DiscountId).Cacheable().SingleOrDefault();
        }

        public void Add(Discount discount)
        {
            _session.Transact(
                session =>
                session.Save(discount));
        }

        public void Save(Discount discount, DiscountLimitation discountLimitation, DiscountApplication discountApplication)
        {
            _session.Transact(session => session.SaveOrUpdate(discount));
        }

        public void Delete(Discount discount)
        {
            _session.Transact(session => session.Delete(discount));
        }
    }
}