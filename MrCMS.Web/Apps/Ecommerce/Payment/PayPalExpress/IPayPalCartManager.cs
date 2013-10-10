using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Website;
using PayPal.PayPalAPIInterfaceService.Model;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public interface IPayPalCartManager
    {
        void UpdateCart(GetExpressCheckoutDetailsResponseDetailsType details);
    }

    public interface IPayPalIPNService
    {
        void HandleIPN(string ipnData);
    }
    
    public class PayPalIPNService : IPayPalIPNService
    {
        private readonly IOrderService _orderService;
        private readonly IOrderNoteService _orderNoteService;
        private readonly IOrderRefundService _orderRefundService;
        private readonly PayPalExpressCheckoutSettings _payPalExpressCheckoutSettings;

        public PayPalIPNService(IOrderService orderService, IOrderNoteService orderNoteService, IOrderRefundService orderRefundService, PayPalExpressCheckoutSettings payPalExpressCheckoutSettings)
        {
            _orderService = orderService;
            _orderNoteService = orderNoteService;
            _orderRefundService = orderRefundService;
            _payPalExpressCheckoutSettings = payPalExpressCheckoutSettings;
        }

        public void HandleIPN(string ipnData)
        {
            Dictionary<string, string> values;
            if (VerifyIPN(ipnData, out values))
            {
                #region values
                decimal total = decimal.Zero;
                try
                {
                    total = decimal.Parse(values["mc_gross"], new CultureInfo("en-US"));
                }
                catch { }

                string payer_status = string.Empty;
                values.TryGetValue("payer_status", out payer_status);
                string payment_status = string.Empty;
                values.TryGetValue("payment_status", out payment_status);
                string pending_reason = string.Empty;
                values.TryGetValue("pending_reason", out pending_reason);
                string mc_currency = string.Empty;
                values.TryGetValue("mc_currency", out mc_currency);
                string txn_id = string.Empty;
                values.TryGetValue("txn_id", out txn_id);
                string txn_type = string.Empty;
                values.TryGetValue("txn_type", out txn_type);
                string rp_invoice_id = string.Empty;
                values.TryGetValue("rp_invoice_id", out rp_invoice_id);
                string payment_type = string.Empty;
                values.TryGetValue("payment_type", out payment_type);
                string payer_id = string.Empty;
                values.TryGetValue("payer_id", out payer_id);
                string receiver_id = string.Empty;
                values.TryGetValue("receiver_id", out receiver_id);
                string invoice = string.Empty;
                values.TryGetValue("invoice", out invoice);
                string payment_fee = string.Empty;
                values.TryGetValue("payment_fee", out payment_fee);

                #endregion

                var sb = new StringBuilder();
                sb.AppendLine("Paypal IPN:");
                foreach (KeyValuePair<string, string> kvp in values)
                {
                    sb.AppendLine(kvp.Key + ": " + kvp.Value);
                }

                var newPaymentStatus = GetPaymentStatus(payment_status, pending_reason);
                sb.AppendLine("New payment status: " + newPaymentStatus);



                string orderNumber = string.Empty;
                values.TryGetValue("custom", out orderNumber);
                Guid orderNumberGuid = Guid.Empty;
                try
                {
                    orderNumberGuid = new Guid(orderNumber);
                }
                catch
                {
                }

                var order = _orderService.GetByGuid(orderNumberGuid);
                if (order != null)
                {

                    //order note
                    _orderNoteService.Save(new OrderNote
                    {
                        Note = sb.ToString(),
                        Order = order,
                        ShowToClient = false
                    });
                    _orderService.Save(order);

                    switch (newPaymentStatus)
                    {
                        case PaymentStatus.Pending:
                            break;
                        case PaymentStatus.Paid:
                            _orderService.MarkAsPaid(order);
                            break;
                        case PaymentStatus.Refunded:
                            _orderRefundService.Add(new OrderRefund
                                                        {
                                                            Order = order,
                                                            Amount = order.TotalAfterRefunds
                                                        }, order);
                            break;
                        case PaymentStatus.Voided:
                            _orderService.MarkAsVoided(order);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    CurrentRequestData.ErrorSignal.Raise(new Exception("PayPal IPN. Order is not found", new Exception(sb.ToString())));
                }

            }
            else
            {
                CurrentRequestData.ErrorSignal.Raise(new Exception("PayPal IPN failed.", new Exception(ipnData)));
            }
        }
        /// <summary>
        /// Verifies IPN
        /// </summary>
        /// <param name="formString">Form string</param>
        /// <param name="values">Values</param>
        /// <returns>Result</returns>
        public bool VerifyIPN(string ipnData, out Dictionary<string, string> values)
        {
            var req = (HttpWebRequest)WebRequest.Create(GetPaypalUrl());
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            string formContent = string.Format("{0}&cmd=_notify-validate", ipnData);
            req.ContentLength = formContent.Length;

            using (var sw = new StreamWriter(req.GetRequestStream(), Encoding.ASCII))
            {
                sw.Write(formContent);
            }

            string response = null;
            using (var sr = new StreamReader(req.GetResponse().GetResponseStream()))
            {
                response = HttpUtility.UrlDecode(sr.ReadToEnd());
            }
            bool success = response.Trim().Equals("VERIFIED", StringComparison.OrdinalIgnoreCase);

            values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string l in ipnData.Split('&'))
            {
                string line = l.Trim();
                int equalPox = line.IndexOf('=');
                if (equalPox >= 0)
                    values.Add(line.Substring(0, equalPox), line.Substring(equalPox + 1));
            }

            return success;
        }
        /// <summary>
        /// Gets Paypal URL
        /// </summary>
        /// <returns></returns>
        private string GetPaypalUrl()
        {
            return _payPalExpressCheckoutSettings.IsLive ? "https://www.paypal.com/us/cgi-bin/webscr" :
                       "https://www.sandbox.paypal.com/us/cgi-bin/webscr";
        }
        /// <summary>
        /// Gets a payment status
        /// </summary>
        /// <param name="paymentStatus">PayPal payment status</param>
        /// <param name="pendingReason">PayPal pending reason</param>
        /// <returns>Payment status</returns>
        private static PaymentStatus GetPaymentStatus(string paymentStatus, string pendingReason)
        {
            var result = PaymentStatus.Pending;

            if (paymentStatus == null)
                paymentStatus = string.Empty;

            if (pendingReason == null)
                pendingReason = string.Empty;

            switch (paymentStatus.ToLowerInvariant())
            {
                case "pending":
                    switch (pendingReason.ToLowerInvariant())
                    {
                        //case "authorization":
                        //    result = PaymentStatus.Authorized;
                        //    break;
                        default:
                            result = PaymentStatus.Pending;
                            break;
                    }
                    break;
                case "processed":
                case "completed":
                case "canceled_reversal":
                    result = PaymentStatus.Paid;
                    break;
                case "denied":
                case "expired":
                case "failed":
                case "voided":
                    result = PaymentStatus.Voided;
                    break;
                case "refunded":
                case "reversed":
                    result = PaymentStatus.Refunded;
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}