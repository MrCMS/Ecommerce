using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IBrandAdminService
    {
        IPagedList<BrandPage> Search(BrandSearchModel searchModel);
        BrandListing GetListingPage();
        bool AnyToMigrate();
        void MigrateBrands();
        List<SelectListItem> GetOptions();
    }
}