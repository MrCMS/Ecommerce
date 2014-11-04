using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
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
    }
}