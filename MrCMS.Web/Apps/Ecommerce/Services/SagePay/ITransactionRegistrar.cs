using System;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.SagePay
{
    public interface ITransactionRegistrar
    {
        /// <summary>
        /// Sends a transaction registration to SagePay and receives a TransactionRegistrationResponse
        /// </summary>
        TransactionRegistrationResponse Send(CartModel cartModel);
    }

    /// <summary>
    /// Represents a transaction registration that is sent to SagePay. 
    /// This should be serialized using the HttpPostSerializer.
    /// </summary>
    public class TransactionRegistration
    {
        public string VPSProtocol { get; set; }
        public string TxType { get; set; }
        public string Vendor { get; set; }
        public string VendorTxCode { get; set; }
        [Format("f2")]
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string NotificationUrl { get; set; }

        public string BillingSurname { get; set; }
        public string BillingFirstNames { get; set; }
        public string BillingAddress1 { get; set; }
        [Optional]
        public string BillingAddress2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingPostcode { get; set; }
        public string BillingCountry { get; set; }
        [Optional]
        public string BillingState { get; set; }
        [Optional]
        public string BillingPhone { get; set; }

        public string DeliverySurname { get; set; }
        public string DeliveryFirstNames { get; set; }
        public string DeliveryAddress1 { get; set; }
        [Optional]
        public string DeliveryAddress2 { get; set; }
        public string DeliveryCity { get; set; }
        public string DeliveryPostcode { get; set; }
        public string DeliveryCountry { get; set; }
        [Optional]
        public string DeliveryState { get; set; }
        [Optional]
        public string DeliveryPhone { get; set; }

        public string CustomerEMail { get; set; }
        public string Basket { get; set; }
        [Optional]
        public string AllowGiftAid { get; set; }
        [Optional]
        public string ApplyAVSCV2 { get; set; }
        [Optional]
        public string Apply3DSecure { get; set; }
        [Optional]
        public string Profile { get; set; }
    }
    /// <summary>
    /// Marks a property as optional when being serialized. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionalAttribute : Attribute
    {
    }

    /// <summary>
    /// Specifies that a property should not be URL Encoded when being serialized by the HttpPostSerialzier
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class UnencodedAttribute : Attribute
    {
    }

    /// <summary>
    /// Specifies a format to use when serializing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FormatAttribute : Attribute
    {
        public string Format { get; private set; }

        public FormatAttribute(string format)
        {
            Format = format;
        }
    }
}