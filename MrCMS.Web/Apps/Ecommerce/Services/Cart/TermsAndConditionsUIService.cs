namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class TermsAndConditionsUIService : ITermsAndConditionsUIService
    {
        private readonly ICartSessionManager _cartSessionManager;
        private readonly IGetUserGuid _getUserGuid;

        public TermsAndConditionsUIService(ICartSessionManager cartSessionManager,IGetUserGuid getUserGuid)
        {
            _cartSessionManager = cartSessionManager;
            _getUserGuid = getUserGuid;
        }

        public void SetAcceptance(bool accept)
        {
            _cartSessionManager.SetSessionValue(CartManager.TermsAndConditionsAcceptedKey, _getUserGuid.UserGuid, accept);
        }
    }
}