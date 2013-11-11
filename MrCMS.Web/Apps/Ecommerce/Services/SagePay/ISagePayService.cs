using MrCMS.Web.Apps.Ecommerce.Models;
using SagePayMvc;

namespace MrCMS.Web.Apps.Ecommerce.Services.SagePay
{
    public interface ISagePayService
    {
        TransactionRegistrationResponse RegisterTransaction(CartModel model);
    }
}