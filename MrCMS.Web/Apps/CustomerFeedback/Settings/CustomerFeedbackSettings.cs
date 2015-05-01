using System;
using System.ComponentModel;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.CustomerFeedback.Settings
{
    public class CustomerFeedbackSettings : SiteSettingsBase
    {
        [DisplayName("Is Enabled?")]
        public bool IsEnabled { get; set; }

        [DisplayName("Notification Email Addresses")]
        public string NotificationEmailAddresses { get; set; }

        [DisplayName("Send Feedback Start Date")]
        public DateTime SendFeedbackStartDate { get; set; }

        [DisplayName("Time After Order To Send Feedback Email")]
        public int TimeAfterOrderToSendFeedbackEmail { get; set; }

        public override bool RenderInSettings
        {
            get { return false; }
        }
    }
}