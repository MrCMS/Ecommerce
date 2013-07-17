using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Models;
using NHibernate;
using MrCMS.Helpers;
using System.Linq;
using System.Linq.Expressions;

namespace MrCMS.Web.Apps.Ecommerce.Services.Shipping
{
    public class ShippingCalculationManager : IShippingCalculationManager
    {
        private readonly ISession _session;

        public ShippingCalculationManager(ISession session)
        {
            _session = session;
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
                                      if(shippingCalculation.Country!=null)
                                          shippingCalculation.Country.ShippingCalculations.Add(shippingCalculation);
                                      session.Save(shippingCalculation);
                                  });
        }

        public void Update(ShippingCalculation shippingCalculation)
        {
            _session.Transact(session => session.Update(shippingCalculation));
        }

        public bool IsValidForAdding(ShippingCalculation shippingCalculation)
        {
            var lowerBound = shippingCalculation.LowerBound;
            var upperBound = shippingCalculation.UpperBound.HasValue ? shippingCalculation.UpperBound.Value : 0;
            var calcs = _session.QueryOver<ShippingCalculation>()
                .Where(x =>
                    x.Id != shippingCalculation.Id &&
                    x.ShippingCriteria == shippingCalculation.ShippingCriteria &&
                    x.Country.Id == shippingCalculation.Country.Id &&
                    x.ShippingMethod.Id == shippingCalculation.ShippingMethod.Id)
                .Cacheable().List();
            if(upperBound>0)
                return !calcs.Any(x => (x.LowerBound <= lowerBound && lowerBound <= x.UpperBound)
                    || (x.LowerBound <= upperBound && (upperBound <= x.UpperBound || x.UpperBound==null)));
            return !calcs.Any(x => (x.LowerBound <= lowerBound && lowerBound <= x.UpperBound) || x.UpperBound==null);
        }

        public void Delete(ShippingCalculation shippingCalculation)
        {
            _session.Transact(session => session.Delete(shippingCalculation));
        }

        public ShippingCalculation Get(int id)
        {
            return _session.QueryOver<ShippingCalculation>().Where(x => x.Id==id).Cacheable().SingleOrDefault();
        }

        public List<SelectListItem> GetAllWhichCanBeUsedForCart(CartModel cart)
        {
            var shippingCalculations = _session.QueryOver<ShippingCalculation>().Cacheable().List();
            return shippingCalculations.Where(x => x.CanBeUsed(cart))
                        .OrderBy(x => x.Country.DisplayOrder)
                        .ThenBy(x => x.ShippingMethod.DisplayOrder)
                        .BuildSelectItemList(item => item.Country.ISOTwoLetterCode + " - " + item.ShippingMethod.Name + " - £" + item.GetPrice(cart)
                        , item => item.Id.ToString(), emptyItemText: null);

        }

    }
}