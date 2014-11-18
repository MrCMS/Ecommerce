using MrCMS.Entities;
using MrCMS.Entities.People;

namespace MrCMS.Web.Apps.Ecommerce.Entities.RewardPoints
{
    public abstract class RewardPointsHistory : SiteEntity
    {
        public virtual string Message { get; set; }
        public virtual User User { get; set; }
        public virtual int Points { get; set; }

        public abstract string DisplayName { get; }
    }
}