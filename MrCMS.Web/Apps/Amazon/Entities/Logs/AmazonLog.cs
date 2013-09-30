using System;
using MrCMS.Entities;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Entities.Logs
{
    public class AmazonLog : SiteEntity
    {
        public virtual AmazonLogType LogType { get; set; }
        public virtual AmazonLogStatus LogStatus { get; set; }

        public virtual AmazonApiSection? ApiSection { get; set; }
        public virtual string ApiOperation { get; set; }

        public virtual AmazonOrder AmazonOrder { get; set; }
        public virtual AmazonListing AmazonListing { get; set; }
        public virtual AmazonListingGroup AmazonListingGroup { get; set; }

        public virtual string ErrorCode { get; set; }
        public virtual string ErrorType { get; set; }
        public virtual string Message { get; set; }
        public virtual string Detail { get; set; }

        public virtual Guid Guid { get; set; }
        public virtual User User { get; set; }
    }
}