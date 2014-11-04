using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class GiftCardSettingsAdminService : IGiftCardSettingsAdminService
    {
        private readonly IConfigurationProvider _configurationProvider;

        public GiftCardSettingsAdminService(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public List<SelectListItem> GetActivationStatusOptions()
        {
            return Enum.GetValues(typeof (ActivateOn)).Cast<ActivateOn>()
                .BuildSelectItemList(@on => @on.ToString().BreakUpString(),
                    @on => @on.ToString(),
                    emptyItem: null);
        }

        public GiftCardSettings GetSettings()
        {
            return _configurationProvider.GetSiteSettings<GiftCardSettings>();
        }

        public void SaveSettings(GiftCardSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
        }
    }
}