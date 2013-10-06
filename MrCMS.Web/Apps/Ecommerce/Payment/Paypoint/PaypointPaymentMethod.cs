using System.ComponentModel;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Paypoint
{
    public class PaypointPaymentMethod : BasePaymentMethod
    {
        public override string Name
        {
            get { return "Pay by card"; }
        }

        public override string SystemName
        {
            get { return "Paypoint"; }
        }

        public override PaymentType PaymentType
        {
            get { return PaymentType.ServiceBased; }
        }

        public override bool Enabled
        {
            get { return MrCMSApplication.Get<PaypointSettings>().Enabled; }
        }

        public override bool CanUse(CartModel cart)
        {
            return !cart.IsPayPalTransaction;
        }
    }

    public class PaypointSettings : SiteSettingsBase
    {
        public override bool RenderInSettings
        {
            get { return false; }
        }

        public bool Enabled { get; set; }

        [DisplayName("3D Secure enabled")]
        public bool ThreeDSecureEnabled { get; set; }
        [DisplayName("Disable 3D secure for Administrators")]
        public bool Admin3DSecureDisable { get; set; }
        [DisplayName("Is live")]
        public bool IsLive { get; set; }
        [DisplayName("Account name")]
        public string AccountName { get; set; }
        [DisplayName("VPN password")]
        public string VPNPassword { get; set; }

        [DisplayName("MPI merchant name")]
        public string MPIMerchantName { get; set; }
        [DisplayName("MPI merchant URL")]
        public string MPIMerchantUrl { get; set; }
        [DisplayName("MPI description")]
        public string MPIDescription { get; set; }
    }
}