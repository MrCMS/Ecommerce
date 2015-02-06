using System;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.RewardPoints;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class AssignRewardPointInfo : IAssignRewardPointInfo, ICartSessionKeyList
    {
        public const string UseRewardPoints = "current.use-reward-points";
        private readonly ICartSessionManager _cartSessionManager;
        private readonly IGetUserRewardPointsBalance _getUserRewardPointsBalance;

        public AssignRewardPointInfo(ICartSessionManager cartSessionManager, IGetUserRewardPointsBalance getUserRewardPointsBalance)
        {
            _cartSessionManager = cartSessionManager;
            _getUserRewardPointsBalance = getUserRewardPointsBalance;
        }

        public CartModel Assign(CartModel cart, Guid userGuid)
        {
            cart.UseRewardPoints = _cartSessionManager.GetSessionValue<bool>(UseRewardPoints, userGuid);
            cart.AvailablePoints = _getUserRewardPointsBalance.GetBalance(cart.User);
            cart.AvailablePointsValue = _getUserRewardPointsBalance.GetBalanceValue(cart.User);
            cart.RewardPointsExchangeRate = _getUserRewardPointsBalance.GetExchangeRate();

            return cart;
        }

        public IEnumerable<string> Keys
        {
            get { yield return UseRewardPoints; }
        }
    }
}