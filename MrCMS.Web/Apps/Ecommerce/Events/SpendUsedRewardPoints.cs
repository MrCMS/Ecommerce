using System;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.RewardPoints;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;
using MrCMS.Web.Apps.Ecommerce.Settings;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Events
{
    public class SpendUsedRewardPoints: IOnOrderPlaced
    {
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly RewardPointSettings _rewardPointSettings;
        private readonly ISession _session;

        public SpendUsedRewardPoints(EcommerceSettings ecommerceSettings,RewardPointSettings rewardPointSettings,ISession session)
        {
            _ecommerceSettings = ecommerceSettings;
            _rewardPointSettings = rewardPointSettings;
            _session = session;
        }

        public void Execute(OrderPlacedArgs args)
        {
            if (!_ecommerceSettings.RewardPointsEnabled)
                return;

            var order = args.Order;

            if (order.User == null)
                return;

            var rewardPointsAppliedAmount = order.RewardPointsAppliedAmount;
            var pointsToSpend = GetPointsToSpend(rewardPointsAppliedAmount);
            if (pointsToSpend <= 0)
                return;

            _session.Transact(session => session.Save(new PointsSpent
            {
                Points = -pointsToSpend,
                User = order.User,
                Order = order,
            }));
        }

        private int GetPointsToSpend(decimal rewardPointsAppliedAmount)
        {
            if (rewardPointsAppliedAmount == decimal.Zero)
                return 0;
            return (int) Math.Ceiling(rewardPointsAppliedAmount/_rewardPointSettings.ExchangeRate);
        }
    }
}