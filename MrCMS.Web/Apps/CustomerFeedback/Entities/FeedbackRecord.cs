using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.CustomerFeedback.Entities
{
    public class FeedbackRecord : SiteEntity
    {
        public virtual Order Order { get; set; }
        public virtual User User { get; set; }
        public virtual IList<FeedbackFacetRecord> FeedbackFacetRecords { get; set; } 
    }
}