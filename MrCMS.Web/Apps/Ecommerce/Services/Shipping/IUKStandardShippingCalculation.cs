namespace MrCMS.Web.Apps.Ecommerce.Services.Shipping
{
    public interface IUKStandardShippingCalculation : IStandardShippingCalculation
    {
        string RestrictedTo { get; set; }
        string ExcludedFrom { get; set; }
    }
}