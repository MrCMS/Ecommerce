using MrCMS.Entities;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Entities.Analytics
{
    public class AmazonApiLog : SiteEntity
    {
        public virtual AmazonApiSection? ApiSection { get; set; }
        public virtual string ApiOperation { get; set; }
        public virtual AmazonApiLogResultStatus Status { get; set; }
    }
}