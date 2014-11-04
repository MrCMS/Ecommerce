using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Entities.GiftCards
{
    public class GiftCard : SiteEntity, IGiftCardUserDetails
    {
        public GiftCard()
        {
            GiftCardUsages = new List<GiftCardUsage>();
        }

        public virtual IList<GiftCardUsage> GiftCardUsages { get; set; }

        [DisplayName("Gift card type")]
        public virtual GiftCardType GiftCardType { get; set; }

        public virtual decimal Value { get; set; }

        public virtual decimal AvailableAmount
        {
            get { return Value - GiftCardUsages.Sum(usage => usage.Amount); }
        }

        [Required]
        public virtual string Code { get; set; }

        [StringLength(100)]
        [DisplayName("Sender Name")]
        public virtual string SenderName { get; set; }

        [StringLength(100)]
        [DisplayName("Sender Email")]
        public virtual string SenderEmail { get; set; }

        [StringLength(100)]
        [DisplayName("Recipient Name")]
        public virtual string RecipientName { get; set; }

        [StringLength(100)]
        [DisplayName("Recipient Email")]
        public virtual string RecipientEmail { get; set; }

        [StringLength(1000)]
        [DisplayName("Message")]
        public virtual string Message { get; set; }

        [DisplayName("Activated?")]
        public virtual GiftCardStatus Status { get; set; }

        [DisplayName("Notification Sent?")]
        public virtual bool NotificationSent { get; set; }

        public virtual OrderLine OrderLine { get; set; }
    }

    public enum GiftCardStatus
    {
        Unactivated,
        Activated,
        Deactivated
    }
}