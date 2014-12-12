using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IRewardPointSettingsAdminService
    {
        RewardPointSettings GetSettings();
        void SaveSettings(RewardPointSettings rewardPointSettings);
    }
}