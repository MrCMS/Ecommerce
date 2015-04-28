using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.RewardPoints;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.RewardPoints;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.UserAccount
{
    public class GetUserRewardPointsStatement : IGetUserRewardPointsStatement
    {
        private readonly ISession _session;
        private readonly IGetUserRewardPointsBalance _getUserRewardPointsBalance;

        public GetUserRewardPointsStatement(ISession session, IGetUserRewardPointsBalance getUserRewardPointsBalance)
        {
            _session = session;
            _getUserRewardPointsBalance = getUserRewardPointsBalance;
        }

        public IPagedList<RewardPointsHistory> Get(User user, int page = 1)
        {
            return
                _session.QueryOver<RewardPointsHistory>()
                    .Where(x => x.User.Id == user.Id)
                    .OrderBy(x => x.CreatedOn)
                    .Desc
                    .Cacheable()
                    .List()
                    .ToPagedList(page);
        }

        public UserRewardPointsModel GetDetails(User user)
        {
            return new UserRewardPointsModel
            {
                Balance = _getUserRewardPointsBalance.GetBalance(user),
                BalanceValue = _getUserRewardPointsBalance.GetBalanceValue(user)
            };
        }
    }
}