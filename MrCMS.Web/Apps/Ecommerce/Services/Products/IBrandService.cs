using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Paging;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IBrandService
    {
        IList<Brand> GetAll();
        IPagedList<Brand> GetPaged(int pageNum, string search, int pageSize = 10);
        void Add(Brand item);
        void Update(Brand item);
        void Delete(Brand item);
    }
}