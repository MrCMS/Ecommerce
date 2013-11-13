using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.SagePay
{
    public interface ISagePayCartLoader
    {
        CartModel GetCart(string vendorTxCode);
    }
}