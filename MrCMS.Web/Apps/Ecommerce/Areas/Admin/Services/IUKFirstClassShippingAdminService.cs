using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Settings.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IUKFirstClassShippingAdminService
    {
        UKFirstClassShippingSettings GetSettings();
        List<SelectListItem> GetTaxRateOptions();
        void Save(UKFirstClassShippingSettings settings);
        List<UKFirstClassShippingCalculation> GetCalculations();
    }
}