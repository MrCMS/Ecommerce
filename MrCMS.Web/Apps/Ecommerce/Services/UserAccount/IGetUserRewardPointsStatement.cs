using MrCMS.Entities.People;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.RewardPoints;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.UserAccount
{
    public interface IGetUserRewardPointsStatement
    {
        IPagedList<RewardPointsHistory> Get(User user, int page = 1);
        UserRewardPointsModel GetDetails(User user);
    }
}