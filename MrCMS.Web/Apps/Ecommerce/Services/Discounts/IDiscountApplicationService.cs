using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Services.Discounts
{
    public interface IDiscountApplicationService
    {
        DiscountApplication Get(int id);
    }
}
