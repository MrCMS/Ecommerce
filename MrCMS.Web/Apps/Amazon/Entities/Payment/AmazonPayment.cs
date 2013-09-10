using MrCMS.Entities;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Currencies;

namespace MrCMS.Web.Apps.Amazon.Entities.Payment
{
    public class AmazonPayment : SiteEntity
    {
        public virtual AmazonOrder AmazonOrder { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual decimal Amount { get; set; }
    }
}