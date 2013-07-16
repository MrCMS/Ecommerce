using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Models;
using NHibernate;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Services.Shipping
{
    public class ShippingMethodManager : IShippingMethodManager
    {
        private readonly ISession _session;

        public ShippingMethodManager(ISession session)
        {
            _session = session;
        }

        public IList<ShippingMethod> GetAll()
        {
            return _session.QueryOver<ShippingMethod>().Cacheable().List();
        }

        public ShippingMethod Get(int id)
        {
            return _session.QueryOver<ShippingMethod>().Where(x => x.Id == id).Cacheable().SingleOrDefault();
        }

        public List<SelectListItem> GetOptions()
        {
            return GetAll().BuildSelectItemList(rate => rate.Name, rate => rate.Id.ToString(), emptyItemText: "None");
        }

        public void Add(ShippingMethod ShippingMethod)
        {
            _session.Transact(session =>
            {
                session.Save(ShippingMethod);
            });
        }

        public void Update(ShippingMethod ShippingMethod)
        {
            _session.Transact(session => session.Update(ShippingMethod));
        }

        public void Delete(ShippingMethod ShippingMethod)
        {
            _session.Transact(session => session.Delete(ShippingMethod));
        }
    }
}