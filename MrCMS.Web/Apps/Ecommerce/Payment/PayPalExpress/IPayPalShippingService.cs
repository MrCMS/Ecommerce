namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public interface IPayPalShippingService
    {
        string GetRequireConfirmedShippingAddress();
        string GetNoShipping();
    }
}