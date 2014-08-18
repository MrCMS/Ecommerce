using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IGiftCardSettingsAdminService
    {
        List<SelectListItem> GetActivationStatusOptions();
        GiftCardSettings GetSettings();
        void SaveSettings(GiftCardSettings settings);
    }
}