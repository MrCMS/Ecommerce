using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using NHibernate;
using MrCMS.Helpers;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Services.Shipping
{
    public class ShippingCalculationManager : IShippingCalculationManager
    {
        private readonly ISession _session;

        public ShippingCalculationManager(ISession session)
        {
            _session = session;
        }

        public IList<ShippingCalculation> GetAll()
        {
            return _session.QueryOver<ShippingCalculation>().OrderBy(x => x.Name).Asc.Cacheable().List();
        }

        public ShippingCalculation Get(int id)
        {
            return _session.QueryOver<ShippingCalculation>().Where(x => x.Id == id).Cacheable().SingleOrDefault();
        }

        public List<SelectListItem> GetCriteriaOptions()
        {
            return
                Enum.GetValues(typeof (ShippingCriteria))
                    .Cast<ShippingCriteria>()
                    .BuildSelectItemList(GetDescription,
                                         criteria => criteria.ToString(), emptyItem: null);
        }

        private static string GetDescription<T>(T item) where T : struct
        {
            FieldInfo field = typeof(T).GetField(item.ToString());
            return field.GetCustomAttributes(typeof(DescriptionAttribute), false)
                        .Cast<DescriptionAttribute>()
                        .Select(x => x.Description)
                        .FirstOrDefault();
        }

        public void Add(ShippingCalculation shippingCalculation)
        {
            _session.Transact(session =>
                                  {
                                      if (shippingCalculation.ShippingMethod != null)
                                          shippingCalculation.ShippingMethod.ShippingCalculations.Add(shippingCalculation);
                                      session.Save(shippingCalculation);
                                  });
        }

        public void Update(ShippingCalculation shippingCalculation)
        {
            _session.Transact(session => session.Update(shippingCalculation));
        }

        public void Delete(ShippingCalculation shippingCalculation)
        {
            _session.Transact(session => session.Delete(shippingCalculation));
        }

        public List<SelectListItem> GetOptions()
        {
            return GetAll().BuildSelectItemList(rate => rate.Name, rate => rate.Id.ToString(), emptyItemText: null);
        }
    }
}