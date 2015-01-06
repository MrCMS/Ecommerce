using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class UKFirstClassShippingCalculationAdminService : IUKFirstClassShippingCalculationAdminService
    {
        private readonly IGetShippingCriteriaOptions _getShippingCriteriaOptions;
        private readonly ISession _session;

        public UKFirstClassShippingCalculationAdminService(ISession session, IGetShippingCriteriaOptions getShippingCriteriaOptions)
        {
            _session = session;
            _getShippingCriteriaOptions = getShippingCriteriaOptions;
        }

        public List<SelectListItem> GetCriteriaOptions()
        {
            return _getShippingCriteriaOptions.Get();
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