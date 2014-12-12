using System.ComponentModel;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Settings
{
    public class DashBoardRevenueSettings : SiteSettingsBase
    {
        [DisplayName("X Days to be shown in Dashboard Revenue")]
        public virtual int Days { get; set; }

        public DashBoardRevenueSettings()
        {
            Days = 7;
        }
    }
}