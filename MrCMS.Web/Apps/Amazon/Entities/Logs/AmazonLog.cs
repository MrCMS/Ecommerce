using System;
using MrCMS.Entities;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Entities.Logs
{
    public class AmazonLog : SiteEntity
    {
        public virtual AmazonLogType Type { get; set; }
        public virtual AmazonLogStatus Status { get; set; }

        public virtual AmazonApiSection? ApiSection { get; set; }
        public virtual string ApiOperation { get; set; }

        public virtual AmazonOrder AmazonOrder { get; set; }
        public virtual AmazonListing AmazonListing { get; set; }

        public virtual Guid Guid { get; set; }
        public virtual string Message { get; set; }
        public virtual string Details { get; set; }
    }
}