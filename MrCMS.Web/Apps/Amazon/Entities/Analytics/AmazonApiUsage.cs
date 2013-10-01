using MrCMS.Entities;
using System;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Entities.Analytics
{
    public class AmazonApiUsage : SiteEntity
    {
        public AmazonApiUsage()
        {
            NoOfCalls = 0;
            Day = CurrentRequestData.Now.Date;
        }
        public virtual AmazonApiSection? ApiSection { get; set; }
        public virtual string ApiOperation { get; set; }
        public virtual int NoOfCalls { get; set; }
        public virtual DateTime Day { get; set; }
    }
}