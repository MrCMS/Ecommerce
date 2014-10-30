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
    public class UKFirstClassShippingCalculationAdminService : IUKFirstClassShippingCalculationAdminService
    {
        private readonly ISession _session;

        public UKFirstClassShippingCalculationAdminService(ISession session)
        {
            _session = session;
        }

        public List<SelectListItem> GetCriteriaOptions()
        {
            return Enum.GetValues(typeof (ShippingCriteria)).Cast<ShippingCriteria>()
                .BuildSelectItemList(criteria => criteria.GetDescription(),
                    criteria => criteria.ToString(), emptyItem: null);
        }

        public void Add(UKFirstClassShippingCalculation calculation)
        {
            _session.Transact(session => session.Save(calculation));
        }

        public void Update(UKFirstClassShippingCalculation calculation)
        {
            _session.Transact(session => session.Update(calculation));
        }

        public void Delete(UKFirstClassShippingCalculation calculation)
        {
            _session.Transact(session => session.Delete(calculation));
        }
    }
}