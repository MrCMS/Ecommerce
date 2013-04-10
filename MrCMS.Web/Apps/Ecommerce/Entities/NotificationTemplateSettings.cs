using System;
using System.Collections.Generic;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Ecommerce.Entities;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Entities;
namespace MrCMS.Web.Apps.Ecommerce.Entities
{
    public class NotificationTemplateSettings : SiteEntity
    {
        public virtual string Name { get; set; }

        public virtual string OrderConfirmationTemplate { get; set; }
        public virtual string ShippingNotificationTemplate { get; set; }
        public virtual string CancelledNotificationTemplate { get; set; }

        public virtual string Emails { get; set; }
        private string[] EmailList { get; set; }
        public virtual string OwnerTemplate { get; set; }
    }
}