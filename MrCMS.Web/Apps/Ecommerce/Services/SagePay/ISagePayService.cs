using System;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.SagePay;

namespace MrCMS.Web.Apps.Ecommerce.Services.SagePay
{
    public interface ISagePayService
    {
        TransactionRegistrationResponse RegisterTransaction(CartModel model);
        string GetSecurityKey(Guid userGuid);
        decimal GetCartTotal(Guid userGuid);
        void SetResponse(Guid userGuid, SagePayResponse response);
        SagePayResponse GetResponse(Guid userGuid);
        void ResetSessionInfo(Guid userGuid);
        void SetFailureDetails(Guid userGuid, FailureDetails failureDetails);
        FailureDetails GetFailureDetails(Guid userGuid);
    }

    /// <summary>
    /// Object that represents a notification POST from SagePay
    /// 
    /// </summary>
    [ModelBinder(typeof(SagePayBinder))]
    public class SagePayResponse
    {
        public ResponseType Status { get; set; }

        public string VendorTxCode { get; set; }

        public string VPSTxId { get; set; }

        public string VPSSignature { get; set; }

        public string StatusDetail { get; set; }

        public string TxAuthNo { get; set; }

        public string AVSCV2 { get; set; }

        public string AddressResult { get; set; }

        public string PostCodeResult { get; set; }

        public string CV2Result { get; set; }

        public string GiftAid { get; set; }

        public string ThreeDSecureStatus { get; set; }

        public string CAVV { get; set; }

        public string AddressStatus { get; set; }

        public string PayerStatus { get; set; }

        public string CardType { get; set; }

        public string Last4Digits { get; set; }

        public string DeclineCode { get; set; }

        public string ExpiryDate { get; set; }

        public string FraudResponse { get; set; }

        public string BankAuthCode { get; set; }

        /// <summary>
        /// Was the transaction successful?
        /// 
        /// </summary>
        public virtual bool WasTransactionSuccessful
        {
            get
            {
                if (this.Status != ResponseType.Ok && this.Status != ResponseType.Authenticated)
                    return this.Status == ResponseType.Registered;
                else
                    return true;
            }
        }

        public decimal CartTotal { get; set; }

        /// <summary>
        /// Is the signature valid
        /// 
        /// </summary>
        public virtual bool IsSignatureValid(string securityKey, string vendorName)
        {
            return this.GenerateSignature(securityKey, vendorName) == this.VPSSignature;
        }

        /// <summary>
        /// Generates the VPS Signature from the parameters of the POST.
        /// 
        /// </summary>
        public virtual string GenerateSignature(string securityKey, string vendorName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(this.VPSTxId);
            stringBuilder.Append(this.VendorTxCode);
            stringBuilder.Append(((object)this.Status).ToString().ToUpper());
            stringBuilder.Append(this.TxAuthNo);
            stringBuilder.Append(vendorName);
            stringBuilder.Append(this.AVSCV2);
            stringBuilder.Append(securityKey);
            stringBuilder.Append(this.AddressResult);
            stringBuilder.Append(this.PostCodeResult);
            stringBuilder.Append(this.CV2Result);
            stringBuilder.Append(this.GiftAid);
            stringBuilder.Append(this.ThreeDSecureStatus);
            stringBuilder.Append(this.CAVV);
            stringBuilder.Append(this.AddressStatus);
            stringBuilder.Append(this.PayerStatus);
            stringBuilder.Append(this.CardType);
            stringBuilder.Append(this.Last4Digits);
            stringBuilder.Append(this.DeclineCode);
            stringBuilder.Append(this.ExpiryDate);
            stringBuilder.Append(this.FraudResponse);
            stringBuilder.Append(this.BankAuthCode);
            return FormsAuthentication.HashPasswordForStoringInConfigFile(((object)stringBuilder).ToString(), "MD5");
        }
    }
    /// <summary>
    /// IModelBinder implementation for deserializing a notification post into a SagePayResponse object.
    /// 
    /// </summary>
    public class SagePayBinder : IModelBinder
    {
        private const string Status = "Status";
        private const string VendorTxCode = "VendorTxCode";
        private const string VPSTxId = "VPSTxId";
        private const string VPSSignature = "VPSSignature";
        private const string StatusDetail = "StatusDetail";
        private const string TxAuthNo = "TxAuthNo";
        private const string AVSCV2 = "AVSCV2";
        private const string AddressResult = "AddressResult";
        private const string PostCodeResult = "PostCodeResult";
        private const string CV2Result = "CV2Result";
        private const string GiftAid = "GiftAid";
        private const string ThreeDSecureStatus = "3DSecureStatus";
        private const string CAVV = "CAVV";
        private const string AddressStatus = "AddressStatus";
        private const string PayerStatus = "PayerStatus";
        private const string CardType = "CardType";
        private const string Last4Digits = "Last4Digits";
        private const string BankAuthCode = "BankAuthCode";
        private const string DeclineCode = "DeclineCode";
        private const string ExpiryDate = "ExpiryDate";
        private const string FraudResponse = "FraudResponse";

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return new SagePayResponse
                               {
                                   Status = GetStatus(bindingContext.ValueProvider),
                                   VendorTxCode = GetFormField(VendorTxCode, bindingContext.ValueProvider),
                                   VPSTxId = GetFormField(VPSTxId, bindingContext.ValueProvider),
                                   VPSSignature = GetFormField(VPSSignature, bindingContext.ValueProvider),
                                   StatusDetail = GetFormField(StatusDetail, bindingContext.ValueProvider),
                                   TxAuthNo = GetFormField(TxAuthNo, bindingContext.ValueProvider),
                                   AVSCV2 = GetFormField(AVSCV2, bindingContext.ValueProvider),
                                   AddressResult = GetFormField(AddressResult, bindingContext.ValueProvider),
                                   PostCodeResult = GetFormField(PostCodeResult, bindingContext.ValueProvider),
                                   CV2Result = GetFormField(CV2Result, bindingContext.ValueProvider),
                                   GiftAid = GetFormField(GiftAid, bindingContext.ValueProvider),
                                   ThreeDSecureStatus = GetFormField(ThreeDSecureStatus, bindingContext.ValueProvider),
                                   CAVV = GetFormField(CAVV, bindingContext.ValueProvider),
                                   AddressStatus = GetFormField(AddressStatus, bindingContext.ValueProvider),
                                   PayerStatus = GetFormField(PayerStatus, bindingContext.ValueProvider),
                                   CardType = GetFormField(CardType, bindingContext.ValueProvider),
                                   Last4Digits = GetFormField(Last4Digits, bindingContext.ValueProvider),
                                   BankAuthCode = GetFormField(BankAuthCode, bindingContext.ValueProvider),
                                   DeclineCode = GetFormField(DeclineCode, bindingContext.ValueProvider),
                                   ExpiryDate = GetFormField(ExpiryDate, bindingContext.ValueProvider),
                                   FraudResponse = GetFormField(FraudResponse, bindingContext.ValueProvider)
                               };
        }

        private ResponseType GetStatus(IValueProvider valueProvider)
        {
            return ResponseSerializer.ConvertStringToSagePayResponseType(this.GetFormField(Status, valueProvider));
        }

        private string GetFormField(string key, IValueProvider provider)
        {
            ValueProviderResult valueProviderResult = provider.GetValue(key);
            if (valueProviderResult != null)
                return (string)valueProviderResult.ConvertTo(typeof(string));
            else
                return (string)null;
        }
    }
    /// <summary>
    /// Used for deserializing SagePay response data. 
    /// </summary>
    public class ResponseSerializer
    {
        /// <summary>
        /// Deserializes the response into an instance of type T.
        /// </summary>
        public void Deserialize<T>(string input, T objectToDeserializeInto)
        {
            Deserialize(typeof(T), input, objectToDeserializeInto);
        }

        /// <summary>
        /// Deserializes the response into an object of type T.
        /// </summary>
        public T Deserialize<T>(string input) where T : new()
        {
            var instance = new T();
            Deserialize(typeof(T), input, instance);
            return instance;
        }

        /// <summary>
        /// Deserializes the response into an object of the specified type.
        /// </summary>
        public void Deserialize(Type type, string input, object objectToDeserializeInto)
        {
            if (string.IsNullOrEmpty(input)) return;

            var bits = input.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var nameValuePairCombined in bits)
            {
                int index = nameValuePairCombined.IndexOf('=');
                string name = nameValuePairCombined.Substring(0, index);
                string value = nameValuePairCombined.Substring(index + 1);

                var prop = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);

                if (prop == null)
                {
                    throw new InvalidOperationException(string.Format("Could not find a property on Type '{0}' named '{1}'", type.Name,
                                                                      name));
                }

                //TODO: Investigate building a method of defining custom serializers

                object convertedValue;

                if (prop.PropertyType == typeof(ResponseType))
                {
                    convertedValue = ConvertStringToSagePayResponseType(value);
                }
                else
                {
                    convertedValue = Convert.ChangeType(value, prop.PropertyType);
                }

                prop.SetValue(objectToDeserializeInto, convertedValue, null);
            }
        }

        /// <summary>
        /// Deserializes the response into an object of the specified type.
        /// </summary>
        public object Deserialize(Type type, string input)
        {
            var instance = Activator.CreateInstance(type);
            Deserialize(type, input, instance);
            return instance;
        }

        /// <summary>
        /// Utility method for converting a string into a ResponseType. 
        /// </summary>
        public static ResponseType ConvertStringToSagePayResponseType(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                if (input.StartsWith("OK"))
                {
                    return ResponseType.Ok;
                }

                if (input.StartsWith("NOTAUTHED"))
                {
                    return ResponseType.NotAuthed;
                }

                if (input.StartsWith("ABORT"))
                {
                    return ResponseType.Abort;
                }

                if (input.StartsWith("REJECTED"))
                {
                    return ResponseType.Rejected;
                }

                if (input.StartsWith("MALFORMED"))
                {
                    return ResponseType.Malformed;
                }

                if (input.StartsWith("AUTHENTICATED"))
                {
                    return ResponseType.Authenticated;
                }

                if (input.StartsWith("INVALID"))
                {
                    return ResponseType.Invalid;
                }

                if (input.StartsWith("REGISTERED"))
                {
                    return ResponseType.Registered;
                }

                if (input.StartsWith("ERROR"))
                {
                    return ResponseType.Error;
                }
            }
            return ResponseType.Unknown;
        }
    }
}