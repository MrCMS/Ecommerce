namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public interface IShippingCalculationRequest
    {
        decimal Weight { get; }
    }
}