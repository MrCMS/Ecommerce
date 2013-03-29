using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities;
using NHibernate;
using MrCMS.Helpers;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Services
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
            return _session.QueryOver<ShippingCalculation>().OrderBy(x => x.Country).Asc.Cacheable().List();
        }

        public ShippingCalculation Get(int id)
        {
            return _session.QueryOver<ShippingCalculation>().Where(x => x.Id == id).Cacheable().SingleOrDefault();
        }

        public ShippingCalculation GetByCountryId(int countryId)
        {
            return _session.QueryOver<ShippingCalculation>().Where(x => x.Country.Id == countryId).Cacheable().SingleOrDefault();
        }
        public List<SelectListItem> GetCriteriaOptions()
        {
            return
                Enum.GetValues(typeof (ShippingCriteria))
                    .Cast<ShippingCriteria>()
                    .BuildSelectItemList(GetDescription,
                                         criteria => criteria.ToString(), emptyItem: null);

            //List<SelectListItem> criterias = new List<SelectListItem>();
            //criterias.Add(new SelectListItem() { Selected=true, Text="Based on cart weight", Value="1" });
            //criterias.Add(new SelectListItem() { Selected = true, Text = "Based on cart price", Value = "2" });
            //return criterias;
        }

        public static string GetDescription<T>(T item) where T : struct
        {
            FieldInfo field = typeof(T).GetField(item.ToString());
            return field.GetCustomAttributes(typeof(DescriptionAttribute), false)
                        .Cast<DescriptionAttribute>()
                        .Select(x => x.Description)
                        .FirstOrDefault();
        }

        public void Add(ShippingCalculation ShippingCalculation)
        {
            _session.Transact(session =>
                                  {
                                      if (ShippingCalculation.Country != null)
                                          ShippingCalculation.Country.ShippingCalculations.Add(ShippingCalculation);
                                      session.Save(ShippingCalculation);
                                  });
        }

        public void Update(ShippingCalculation ShippingCalculation)
        {
            _session.Transact(session => session.Update(ShippingCalculation));
        }

        public void Delete(ShippingCalculation ShippingCalculation)
        {
            _session.Transact(session => session.Delete(ShippingCalculation));
        }

        public List<SelectListItem> GetOptions()
        {
            return GetAll().BuildSelectItemList(rate => rate.Name, rate => rate.Id.ToString(), emptyItemText: null);
        }
    }
}