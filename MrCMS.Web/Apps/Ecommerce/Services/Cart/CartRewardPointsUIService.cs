namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class CartRewardPointsUIService : ICartRewardPointsUIService
    {
        private readonly ICartSessionManager _cartSessionManager;
        private readonly IGetUserGuid _getUserGuid;

        public CartRewardPointsUIService(ICartSessionManager cartSessionManager, IGetUserGuid getUserGuid)
        {
            _cartSessionManager = cartSessionManager;
            _getUserGuid = getUserGuid;
        }

        public void SetUseRewardPoints(bool useRewardPoints)
        {
            _cartSessionManager.SetSessionValue(AssignRewardPointInfo.UseRewardPoints, _getUserGuid.UserGuid,
                useRewardPoints);
        }
    }
}