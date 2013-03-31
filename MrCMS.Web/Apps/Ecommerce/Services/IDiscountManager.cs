using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IDiscountManager
    {
        IList<Discount> GetAll();
        Discount Get(int id);
        void Add(Discount discount);
        void Save(Discount discount);
        void Delete(Discount discount);
    }
}