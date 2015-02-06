using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class UKCourierShippingCalculationAdminService : IUKCourierShippingCalculationAdminService
    {
        private readonly IGetShippingCriteriaOptions _getShippingCriteriaOptions;
        private readonly ISession _session;

        public UKCourierShippingCalculationAdminService(ISession session,
            IGetShippingCriteriaOptions getShippingCriteriaOptions)
        {
            _session = session;
            _getShippingCriteriaOptions = getShippingCriteriaOptions;
        }

        public List<SelectListItem> GetCriteriaOptions()
        {
            return _getShippingCriteriaOptions.Get();
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