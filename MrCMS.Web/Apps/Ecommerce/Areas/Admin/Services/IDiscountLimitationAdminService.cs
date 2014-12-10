using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IDiscountLimitationAdminService
    {
        IList<DiscountLimitation> GetLimitations(Discount discount);

        void Add(DiscountLimitation limitation);
        void Update(DiscountLimitation limitation);
        void Delete(DiscountLimitation limitation);

        List<SelectListItem> GetTypeOptions();
    }
}