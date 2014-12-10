using System.Collections.Generic;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Services.Discounts
{
    public interface IDiscountManager
    {
        IList<Discount> GetAll();
        Discount Get(int discountId);
        Discount GetByCode(string code);
        void Add(Discount discount);
        void Save(Discount discount);
        void Delete(Discount discount);
        DiscountApplication GetApplication(Discount discount, string applicationType);
        DiscountLimitation GetLimitation(Discount discount, string limitationType);
        bool IsUniqueCode(string code, int? id = null);
        IPagedList<Discount> Search(DiscountSearchQuery searchQuery);
    }
}