using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class ClearCartSessionKeys : IClearCartSessionKeys
    {
        private readonly ICartSessionManager _cartSessionManager;
        private readonly IEnumerable<ICartSessionKeyList> _sessionKeyLists;
        private readonly IGetUserGuid _getUserGuid;

        public ClearCartSessionKeys(ICartSessionManager cartSessionManager, IEnumerable<ICartSessionKeyList> sessionKeyLists, IGetUserGuid getUserGuid)
        {
            _cartSessionManager = cartSessionManager;
            _sessionKeyLists = sessionKeyLists;
            _getUserGuid = getUserGuid;
        }

        public void Clear()
        {

            CartKeys.ForEach(s => _cartSessionManager.RemoveValue(s, _getUserGuid.UserGuid));

        }

        public IEnumerable<string> CartKeys
        {
            get
            {
                foreach (string key in _sessionKeyLists.SelectMany(keyList => keyList.Keys))
                {
                    yield return key;
                }
            }
        }
    }
}