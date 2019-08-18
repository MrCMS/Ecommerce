using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class BrandListingWebpageTreeNav : WebpageTreeNavListing<BrandListing>
    {
        public override AdminTree GetTree(int? id)
        {
            return new AdminTree();
        }

        public override bool HasChildren(int id)
        {
            return false;
        }
    }
}