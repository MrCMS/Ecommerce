using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class ClickAndCollectAdminService : IClickAndCollectAdminService
    {
        private readonly IConfigurationProvider _configurationProvider;

        public ClickAndCollectAdminService(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public ClickAndCollectSettings GetSettings()
        {
            return _configurationProvider.GetSiteSettings<ClickAndCollectSettings>();
        }

        public void Save(ClickAndCollectSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
        }
    }
}