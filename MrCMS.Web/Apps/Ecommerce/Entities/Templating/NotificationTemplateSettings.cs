using MrCMS.Entities;
using System.ComponentModel;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Templating
{
    public class MessageTemplateSettings : SiteEntity
    {
        public virtual string OrderConfirmationTemplate { get; set; }
        public virtual string ShippingMessageTemplate { get; set; }
        public virtual string CancelledMessageTemplate { get; set; }
        public virtual string OutOfStockMessageTemplate { get; set; }

        public virtual string Emails { get; set; }
        [DisplayName("Template")]
        public virtual string OwnerTemplate { get; set; }
    }
}