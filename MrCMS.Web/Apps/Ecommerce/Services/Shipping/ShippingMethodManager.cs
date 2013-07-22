using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
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
            return _session.QueryOver<ShippingMethod>().OrderBy(x=>x.DisplayOrder).Asc.Cacheable().List();
        }

        public ShippingMethod Get(int id)
        {
            return _session.QueryOver<ShippingMethod>().Where(x => x.Id == id).Cacheable().SingleOrDefault();
        }

        public List<SelectListItem> GetOptions()
        {
            return GetAll().BuildSelectItemList(rate => rate.Name, rate => rate.Id.ToString(), emptyItemText: null);
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

        public void UpdateDisplayOrder(IList<SortItem> options)
        {
            _session.Transact(session => options.ForEach(item =>
            {
                var formItem = session.Get<ShippingMethod>(item.Id);
                formItem.DisplayOrder = item.Order;
                session.Update(formItem);
            }));
        }
    }
}