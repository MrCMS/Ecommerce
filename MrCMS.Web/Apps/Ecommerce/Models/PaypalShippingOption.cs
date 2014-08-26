namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class PaypalShippingOption
    {
        public string DisplayName { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal InsuranceAmount { get; set; }
        public bool Default { get; set; }
        public string Label { get; set; }
    }
}