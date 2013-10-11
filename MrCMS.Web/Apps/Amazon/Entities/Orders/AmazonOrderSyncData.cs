using MrCMS.Entities;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Entities.Orders
{
    public class AmazonOrderSyncData : SiteEntity
    {
        public virtual string OrderId { get; set; }
        public virtual AmazonOrder AmazonOrder { get; set; }
        public virtual SyncAmazonOrderStatus Status { get; set; }
        public virtual SyncAmazonOrderOperation Operation { get; set; }
        public virtual string Data { get; set; }
    }
}