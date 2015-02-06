using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.RewardPoints;
using MrCMS.Web.Apps.Ecommerce.Settings;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Events
{
    public class UndoRewardPointUsage : IOnUpdated<Order>
    {
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly RewardPointSettings _rewardPointSettings;
        private readonly ISession _session;

        public UndoRewardPointUsage(EcommerceSettings ecommerceSettings, RewardPointSettings rewardPointSettings,
            ISession session)
        {
            _ecommerceSettings = ecommerceSettings;
            _rewardPointSettings = rewardPointSettings;
            _session = session;
        }

        public void Execute(OnUpdatedArgs<Order> args)
        {
            if (!_ecommerceSettings.RewardPointsEnabled)
                return;

            Order order = args.Item;

            if (order.User == null)
                return;

            if (order.OrderStatus != _rewardPointSettings.StatusToCancel)
                return;

            RemoveAddedPoints(order);

            ReimburseSpentPoints(order);
        }

        private void RemoveAddedPoints(Order order)
        {
            PointsAwarded pointsAwarded =
                _session.QueryOver<PointsAwarded>().Where(awarded => awarded.Order.Id == order.Id).SingleOrDefault();
            if (pointsAwarded != null)
            {
                bool anyPointsRemoved =
                    _session.QueryOver<PointsRemoved>()
                        .Where(removed => removed.Order.Id == order.Id && removed.RemovalFor.Id == pointsAwarded.Id)
                        .Any();
                if (!anyPointsRemoved)
                {
                    _session.Transact(session => session.Save(new PointsRemoved
                    {
                        Order = order,
                        RemovalFor = pointsAwarded,
                        Points = -pointsAwarded.Points,
                        User = order.User
                    }));
                }
            }
        }

        private void ReimburseSpentPoints(Order order)
        {
            PointsSpent pointsSpent =
                _session.QueryOver<PointsSpent>().Where(awarded => awarded.Order.Id == order.Id).SingleOrDefault();
            if (pointsSpent != null)
            {
                bool anyPointsReimbursed =
                    _session.QueryOver<PointsReimbursed>()
                        .Where(removed => removed.Order.Id == order.Id && removed.ReimbursmentFor.Id == pointsSpent.Id)
                        .Any();
                if (!anyPointsReimbursed)
                {
                    _session.Transact(session => session.Save(new PointsReimbursed
                    {
                        Order = order,
                        ReimbursmentFor = pointsSpent,
                        Points = -pointsSpent.Points,
                        User = order.User
                    }));
                }
            }
        }
    }
}