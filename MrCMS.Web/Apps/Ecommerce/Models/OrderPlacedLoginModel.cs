using MrCMS.Web.Apps.Core.Models.RegisterAndLogin;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class OrderPlacedLoginModel : LoginModel
    {
        public Order Order { get; set; }
    }
}