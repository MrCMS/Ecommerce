using System.Collections.Generic;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IDiscountAdminService
    {
        IPagedList<Discount> Search(DiscountSearchQuery searchQuery);
        void Add(Discount discount);
        void Update(Discount discount);
        void Delete(Discount discount);
        IList<DiscountUsage> GetUsages(Discount discount);
    }
}