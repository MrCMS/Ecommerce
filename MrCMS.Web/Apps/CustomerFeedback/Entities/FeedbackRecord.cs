using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.CustomerFeedback.Entities
{
    public class FeedbackRecord : SiteEntity
    {
        public FeedbackRecord()
        {
            FeedbackRecords = new List<Feedback>();
        }

        public virtual Order Order { get; set; }
        public virtual User User { get; set; }
        public virtual bool IsCompleted { get; set; }
        public virtual bool IsSent { get; set; }
        public virtual IList<Feedback> FeedbackRecords { get; set; } 
    }
}