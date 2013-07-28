namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public interface IPayPalUrlService
    {
        string GetReturnURL();
        string GetCancelURL();
        string GetExpressCheckoutRedirectUrl(string token);
    }
}