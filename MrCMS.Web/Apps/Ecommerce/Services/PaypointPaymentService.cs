using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.Paypoint;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class PaypointPaymentService : IPaypointPaymentService
    {
        private readonly PaypointSettings _paypointSettings;
        private readonly IPaypointRequestService _paypointRequestService;

        public PaypointPaymentService(PaypointSettings paypointSettings, IPaypointRequestService paypointRequestService)
        {
            _paypointSettings = paypointSettings;
            _paypointRequestService = paypointRequestService;
        }

        public ProcessDetailsResponse ProcessDetails(PaypointPaymentDetailsModel model, string threeDSecureUrl)
        {
            try
            {
                return _paypointSettings.ThreeDSecureEnabled
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

        public IEnumerable<SelectListItem> StartYears()
        {
            return Enumerable.Range(DateTime.Now.Year - 10, 11).BuildSelectItemList(i => i.ToString().Substring(2, 2), emptyItemText: "Year");
        }

        public IEnumerable<SelectListItem> ExpiryYears()
        {
            return Enumerable.Range(DateTime.Now.Year, 11).BuildSelectItemList(i => i.ToString().Substring(2, 2), emptyItemText: "Year");
        }

    }
}