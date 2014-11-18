using System.Collections.Generic;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.RewardPoints;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class UserRewardPointsAdminService : IUserRewardPointsAdminService
    {
        private readonly ISession _session;
        private readonly IGetCurrentUser _getCurrentUser;

        public UserRewardPointsAdminService(ISession session, IGetCurrentUser getCurrentUser)
        {
            _session = session;
            _getCurrentUser = getCurrentUser;
        }

        public IList<RewardPointsHistory> GetAll(User user)
        {
            if (user == null)
                return new List<RewardPointsHistory>();

            return _session.QueryOver<RewardPointsHistory>().Where(history => history.User.Id == user.Id)
                .OrderBy(history => history.CreatedOn).Desc.Cacheable().List();
        }

        public ManualAdjustment GetDefaultAdjustment(User user)
        {
            return new ManualAdjustment
            {
                User = user
            };
        }

        public void AddAdjustment(ManualAdjustment adjustment)
        {
            adjustment.AdjustedBy = _getCurrentUser.Get();
            _session.Transact(session => session.Save(adjustment));
        }
    }
}