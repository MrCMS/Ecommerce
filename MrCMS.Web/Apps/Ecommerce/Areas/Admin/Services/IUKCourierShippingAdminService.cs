using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IUKCourierShippingAdminService
    {
        UKCourierShippingSettings GetSettings();
        List<SelectListItem> GetTaxRateOptions();
        void Save(UKCourierShippingSettings settings);
        List<UKCourierShippingCalculation> GetCalculations();
    }
}