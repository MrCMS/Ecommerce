using System;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.RewardPoints;
using MrCMS.Web.Apps.Ecommerce.Settings;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services.RewardPoints
{
    public class GetUserRewardPointsBalance : IGetUserRewardPointsBalance
    {
        private readonly ISession _session;
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly RewardPointSettings _rewardPointSettings;

        public GetUserRewardPointsBalance(ISession session, EcommerceSettings ecommerceSettings, RewardPointSettings rewardPointSettings)
        {
            _session = session;
            _ecommerceSettings = ecommerceSettings;
            _rewardPointSettings = rewardPointSettings;
        }

        public int GetBalance(User user)
        {
            if (!_ecommerceSettings.RewardPointsEnabled || user == null || !GetBaseQuery(user).Cacheable().Any())
                return 0;

            var balance = GetBaseQuery(user)
                .Select(Projections.Sum<RewardPointsHistory>(x => x.Points))
                .SingleOrDefault<int>();

            var max = Math.Max(0, balance);
            return max < _rewardPointSettings.MinimumUsage
                ? 0
                : max;
        }

        public decimal GetBalanceValue(User user)
        {
            var balance = GetBalance(user);
            if (balance <= 0)
                return decimal.Zero;

            return Math.Floor(_rewardPointSettings.ExchangeRate * balance * 100m) / 100m;
        }

        public decimal GetExchangeRate()
        {
            return _rewardPointSettings.ExchangeRate;
        }

        private IQueryOver<RewardPointsHistory, RewardPointsHistory> GetBaseQuery(User user)
        {
            return _session.QueryOver<RewardPointsHistory>().Where(history => history.User.Id == user.Id);
        }
    }
}