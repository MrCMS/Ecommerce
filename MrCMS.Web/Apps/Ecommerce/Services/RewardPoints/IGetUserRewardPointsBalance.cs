using MrCMS.Entities.People;

namespace MrCMS.Web.Apps.Ecommerce.Services.RewardPoints
{
    public interface IGetUserRewardPointsBalance
    {
        int GetBalance(User user);
        decimal GetBalanceValue(User user);
        decimal GetExchangeRate();
    }
}