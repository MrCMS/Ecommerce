using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class UKCourierShippingCalculationAdminService : IUKCourierShippingCalculationAdminService
    {
        private readonly ISession _session;

        public UKCourierShippingCalculationAdminService(ISession session)
        {
            _session = session;
        }

        public List<SelectListItem> GetCriteriaOptions()
        {
            return Enum.GetValues(typeof (ShippingCriteria)).Cast<ShippingCriteria>()
                .BuildSelectItemList(criteria => EnumHelper.GetDescription(criteria),
                    criteria => criteria.ToString(), emptyItem: null);

        }

        public void Add(UKCourierShippingCalculation calculation)
        {
            _session.Transact(session => session.Save(calculation));
        }

        public void Update(UKCourierShippingCalculation calculation)
        {
            _session.Transact(session => session.Update(calculation));
        }

        public void Delete(UKCourierShippingCalculation calculation)
        {
            _session.Transact(session => session.Delete(calculation));
        }

        public bool IsCalculationValid(CalculationInfo calculationInfo)
        {
            var lowerBound = calculationInfo.LowerBound;
            var upperBound = calculationInfo.UpperBound.HasValue ? calculationInfo.UpperBound.Value : 0;
            var calcs =
                _session.QueryOver<UKCourierShippingCalculation>()
                    .Where(x => x.Id != calculationInfo.Id && x.ShippingCriteria == calculationInfo.ShippingCriteria)
                    .Cacheable().List();
            if (upperBound > 0)
                return !calcs.Any(x => (x.LowerBound <= lowerBound && lowerBound <= x.UpperBound)
                                       || (x.LowerBound <= upperBound && (upperBound <= x.UpperBound || x.UpperBound == null)));
            return !calcs.Any(x => (x.LowerBound <= lowerBound && lowerBound <= x.UpperBound) || x.UpperBound == null);
        }
    }
}