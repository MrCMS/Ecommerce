using System.Collections.Generic;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Ecommerce.Entities.RewardPoints;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IUserRewardPointsAdminService
    {
        IList<RewardPointsHistory> GetAll(User user);
        ManualAdjustment GetDefaultAdjustment(User user);
        void AddAdjustment(ManualAdjustment adjustment);
    }
}