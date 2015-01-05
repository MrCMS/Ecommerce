using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IEcommerceSettingsAdminService
    {
        List<SelectListItem> GetCurrencyOptions();
        EcommerceSettings GetSettings();
        void SaveSettings(EcommerceSettings ecommerceSettings);
        List<SelectListItem> GetDefaultSortOptions();
    }
}