using System.Collections.Generic;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Stats.Entities
{
    public class AnalyticsSession : SiteEntity
    {
        //public virtual Guid Guid { get; set; }
        public virtual AnalyticsUser AnalyticsUser { get; set; }
        public virtual IList<AnalyticsPageView> PageViews { get; set; }
        public virtual string UserAgent { get; set; }
        public virtual string IP { get; set; }
    }
}