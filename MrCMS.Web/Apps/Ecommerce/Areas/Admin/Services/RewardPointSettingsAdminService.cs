using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class RewardPointSettingsAdminService : IRewardPointSettingsAdminService
    {
        private readonly IConfigurationProvider _configurationProvider;

        public RewardPointSettingsAdminService(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public RewardPointSettings GetSettings()
        {
            return _configurationProvider.GetSiteSettings<RewardPointSettings>();
        }

        public void SaveSettings(RewardPointSettings rewardPointSettings)
        {
            _configurationProvider.SaveSettings(rewardPointSettings);
        }
    }
}