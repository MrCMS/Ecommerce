using System;
using System.Collections.Generic;
using System.Web.Routing;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.SagePay;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;

namespace MrCMS.Web.Apps.Ecommerce.Services.SagePay
{
    public class SagePayService : ISagePayService, ICartSessionKeyList
    {
        private readonly ITransactionManager _transactionManager;
        private readonly ICartSessionManager _cartSessionManager;
        private readonly IGetUserGuid _getUserGuid;
        private readonly ICartGuidResetter _cartGuidResetter;
        private const string SagePayEnrolledResponseKey = "current.sagepayenrolledresponse";
        private const string SagePayTransactionResponseKey = "current.sagepaytransactionresponse";
        private const string SagePayFailureDetailsKey = "current.sagepayfailuredetails";

        public SagePayService(ITransactionManager transactionManager, ICartSessionManager cartSessionManager, IGetUserGuid getUserGuid, ICartGuidResetter cartGuidResetter)
        {
            _transactionManager = transactionManager;
            _cartSessionManager = cartSessionManager;
            _getUserGuid = getUserGuid;
            _cartGuidResetter = cartGuidResetter;
        }

        public TransactionRegistrationResponse RegisterTransaction(CartModel model)
        {
            var tryCount = 0;
            while (tryCount++ < 5)
            {
                var transactionRegistrationResponse = _transactionManager.Register(model);
                if (transactionRegistrationResponse.Status == ResponseType.Ok)
                {
                    _cartSessionManager.SetSessionValue(SagePayEnrolledResponseKey, _getUserGuid.UserGuid,
                                                        transactionRegistrationResponse, SessionDataTimeoutDefaults.PaymentInfo);
                    return transactionRegistrationResponse;
                }
                model.CartGuid = _cartGuidResetter.ResetCartGuid(_getUserGuid.UserGuid);
            }
            return new TransactionRegistrationResponse { Status = ResponseType.Error };
        }

        public string GetSecurityKey(Guid userGuid)
        {
            var response = _cartSessionManager.GetSessionValue<TransactionRegistrationResponse>(SagePayEnrolledResponseKey, userGuid);
            return response != null ? response.SecurityKey : null;
        }

        public decimal GetCartTotal(Guid userGuid)
        {
            var response = _cartSessionManager.GetSessionValue<TransactionRegistrationResponse>(SagePayEnrolledResponseKey, userGuid);
            return response != null ? response.CartTotal : decimal.Zero;
        }

        public void SetResponse(Guid userGuid, SagePayResponse response)
        {
            _cartSessionManager.SetSessionValue(SagePayTransactionResponseKey, userGuid, response, SessionDataTimeoutDefaults.PaymentInfo, true);
        }

        public SagePayResponse GetResponse(Guid userGuid)
        {
            return _cartSessionManager.GetSessionValue<SagePayResponse>(SagePayTransactionResponseKey, userGuid,
                                                                        encrypted: true);
        }

        public void ResetSessionInfo(Guid userGuid)
        {
            _cartGuidResetter.ResetCartGuid(userGuid);
            foreach (var key in Keys)
                _cartSessionManager.RemoveValue(key, userGuid);
        }

        public void SetFailureDetails(Guid userGuid, FailureDetails failureDetails)
        {
            _cartSessionManager.SetSessionValue(SagePayFailureDetailsKey, userGuid, failureDetails, SessionDataTimeoutDefaults.PaymentInfo);
        }

        public FailureDetails GetFailureDetails(Guid userGuid)
        {
            var details = _cartSessionManager.GetSessionValue<FailureDetails>(SagePayFailureDetailsKey, userGuid);
            _cartSessionManager.RemoveValue(SagePayFailureDetailsKey, userGuid);
            return details;
        }

        public IEnumerable<string> Keys
        {
            get
            {
                yield return SagePayEnrolledResponseKey;
                yield return SagePayTransactionResponseKey;
                yield return SagePayFailureDetailsKey;
            }
        }
    }
}