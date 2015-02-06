using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.Paypoint;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services.Paypoint
{
    public class PaypointPaymentService : IPaypointPaymentService, ICartSessionKeyList
    {
        private readonly PaypointSettings _paypointSettings;
        private readonly IPaypointRequestService _paypointRequestService;
        private readonly ICartSessionManager _cartSessionManager;
        private readonly IGetUserGuid _getUserGuid;
        private const string PaypointPaymentModelKey = "current.paypoint-model";

        public PaypointPaymentService(PaypointSettings paypointSettings, IPaypointRequestService paypointRequestService,
                                      ICartSessionManager cartSessionManager, IGetUserGuid getUserGuid)
        {
            _paypointSettings = paypointSettings;
            _paypointRequestService = paypointRequestService;
            _cartSessionManager = cartSessionManager;
            _getUserGuid = getUserGuid;
        }

        public ProcessDetailsResponse ProcessDetails(PaypointPaymentDetailsModel model, string threeDSecureUrl)
        {
            try
            {
                var disable3Dsecure = _paypointSettings.Admin3DSecureDisable && CurrentRequestData.CurrentUserIsAdmin;
                return _paypointSettings.ThreeDSecureEnabled && !disable3Dsecure
                           ? _paypointRequestService.Process3DSecureTransaction(model, threeDSecureUrl)
                           : _paypointRequestService.ProcessStandardTransaction(model);
            }
            catch (Exception exception)
            {
                CurrentRequestData.ErrorSignal.Raise(exception);
                return new ProcessDetailsResponse { ErrorOccurred = true };
            }
        }

        public IEnumerable<SelectListItem> GetCardTypes()
        {
            return new List<SelectListItem>
                       {
                           new SelectListItem {Text = "Select card type", Value = null},
                           new SelectListItem {Text = "Visa", Value = "Visa"},
                           new SelectListItem {Text = "Visa Debit", Value = "Delta"},
                           new SelectListItem {Text = "MasterCard", Value = "Master Card"},
                           new SelectListItem {Text = "Maestro", Value = "Maestro"}
                       };
        }

        public IEnumerable<SelectListItem> Months()
        {
            return Enumerable.Range(1, 12).BuildSelectItemList(i => i.ToString("00"), emptyItemText: "Month");
        }

        public IEnumerable<SelectListItem> StartMonths()
        {
            return Enumerable.Range(1, 12).BuildSelectItemList(i => i.ToString().PadLeft(2, '0'), i => i.ToString(), emptyItemText: "Month");
        }

        public IEnumerable<SelectListItem> StartYears()
        {
            return Enumerable.Range(DateTime.Now.Year - 10, 11).BuildSelectItemList(i => i.ToString().Substring(2, 2), emptyItemText: "Year");
        }

        public IEnumerable<SelectListItem> ExpiryMonths()
        {
            return Enumerable.Range(1, 12).BuildSelectItemList(i => i.ToString().PadLeft(2, '0'), i => i.ToString(), emptyItemText: "Month");
        }

        public IEnumerable<SelectListItem> ExpiryYears()
        {
            return Enumerable.Range(DateTime.Now.Year, 11).BuildSelectItemList(i => i.ToString().Substring(2, 2), emptyItemText: "Year");
        }

        public ProcessDetailsResponse Handle3DSecureResponse(FormCollection formCollection)
        {
            return _paypointRequestService.Handle3DSecureResponse(formCollection);
        }

        public void SetModel(PaypointPaymentDetailsModel model)
        {
            _cartSessionManager.SetSessionValue(PaypointPaymentModelKey, _getUserGuid.UserGuid, model, SessionDataTimeoutDefaults.PaymentInfo, true);
        }

        public PaypointPaymentDetailsModel GetModel()
        {
            return _cartSessionManager.GetSessionValue<PaypointPaymentDetailsModel>(PaypointPaymentModelKey, _getUserGuid.UserGuid,
                                                                                    encrypted: true);
        }

        public IEnumerable<string> Keys { get { yield return PaypointPaymentModelKey; } }
    }
}