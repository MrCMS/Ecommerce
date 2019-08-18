using System.Threading.Tasks;
using MrCMS.Models.Auth;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public interface IOrderPlacedService
    {
        EmailRegistrationStatus GetRegistrationStatus(string orderEmail);
        LoginAndAssociateOrderResult LoginAndAssociateOrder(LoginModel model, Order order);
        Task<RegisterAndAssociateOrderResult> RegisterAndAssociateOrder(RegisterModel model, Order order);
        bool UpdateAnalytics(Order order);
    }
}