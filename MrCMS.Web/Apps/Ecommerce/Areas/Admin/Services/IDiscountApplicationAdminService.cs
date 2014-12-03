using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IDiscountApplicationAdminService
    {
        List<SelectListItem> GetTypeOptions();

        IList<DiscountApplication> GetApplications(Discount discount);
        
        void Add(DiscountApplication application);
        void Update(DiscountApplication application);
        void Delete(DiscountApplication application);
    }
}