using MrCMS.Entities;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.CustomerFeedback.Entities
{
    public class CorrespondenceRecord : SiteEntity
    {
        public virtual Order Order { get; set; }
        public virtual User User { get; set; }
        public virtual CorrespondenceDirection CorrespondenceDirection { get; set; }
        public virtual string MessageInfo { get; set; }
    }
}