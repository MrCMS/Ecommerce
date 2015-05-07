using System;
using System.ComponentModel;
using MrCMS.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.CustomerFeedback.Settings
{
    public class CustomerFeedbackSettings : SiteSettingsBase
    {
        public CustomerFeedbackSettings()
        {
            TimeAfterOrderToSendFeedbackEmail = 7;
            SendFeedbackStartDate = CurrentRequestData.Now;
        }

        [DisplayName("Is Enabled?")]
        public bool IsEnabled { get; set; }

        [DisplayName("Item Feedback Enabled?")]
        public bool ItemFeedbackEnabled { get; set; }

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