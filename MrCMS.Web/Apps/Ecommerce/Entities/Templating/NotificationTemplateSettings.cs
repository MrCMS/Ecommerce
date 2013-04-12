using MrCMS.Entities;
using System.ComponentModel;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Templating
{
    public class NotificationTemplateSettings : SiteEntity
    {
        public virtual string Name { get; set; }

        public virtual string OrderConfirmationTemplate { get; set; }
        public virtual string ShippingNotificationTemplate { get; set; }
        public virtual string CancelledNotificationTemplate { get; set; }

        public virtual string Emails { get; set; }
        private string[] EmailList { get; set; }
        [DisplayName("Template")]
        public virtual string OwnerTemplate { get; set; }
    }
}