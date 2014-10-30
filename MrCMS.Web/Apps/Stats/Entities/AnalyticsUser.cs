using System;
using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Entities.People;

namespace MrCMS.Web.Apps.Stats.Entities
{
    public class AnalyticsUser : SiteEntity
    {
        public virtual Guid Guid { get; set; }
        public virtual User User { get; set; }
        public virtual IList<AnalyticsSession> AnalyticsSessions { get; set; }
    }
}