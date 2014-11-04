using MrCMS.Entities;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Stats.Entities
{
    public class AnalyticsPageView : SiteEntity
    {
        public virtual Webpage Webpage { get; set; }
        public virtual AnalyticsSession AnalyticsSession { get; set; }
        public virtual string Url { get; set; }
    }
}