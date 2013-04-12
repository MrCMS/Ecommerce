using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Services.Discounts
{
    public interface IDiscountManager
    {
        IList<Discount> GetAll();
        Discount Get(int id);
        void Add(Discount discount);
        void Save(Discount discount, DiscountLimitation discountLimitation, DiscountApplication discountApplication);
        void Delete(Discount discount);
    }
}