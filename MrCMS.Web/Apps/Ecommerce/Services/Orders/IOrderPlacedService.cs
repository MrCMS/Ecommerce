using System.Threading.Tasks;
using MrCMS.Web.Apps.Core.Models.RegisterAndLogin;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public interface IOrderPlacedService
    {
        EmailRegistrationStatus GetRegistrationStatus(string orderEmail);
        Task<LoginAndAssociateOrderResult> LoginAndAssociateOrder(LoginModel model, Order order);
        Task<RegisterAndAssociateOrderResult> RegisterAndAssociateOrder(RegisterModel model, Order order);
    }
}