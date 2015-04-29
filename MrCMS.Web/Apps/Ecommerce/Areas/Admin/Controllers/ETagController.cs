using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.ETags;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ETagController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IETagAdminService _eTagAdminService;

        public ETagController(IETagAdminService eTagAdminService)
        {
            _eTagAdminService = eTagAdminService;
        }

        [MrCMSACLRule(typeof(ETagACL), ETagACL.List)]
        public ViewResult Index(ETagSearchQuery searchQuery)
        {
            ViewData["results"] = _eTagAdminService.Search(searchQuery);
            return View(searchQuery);
        }

        [HttpGet]
        [MrCMSACLRule(typeof(ETagACL), ETagACL.Add)]
        public PartialViewResult Add()
        {
            return PartialView();
        }

        [HttpPost]
        [ActionName("Add")]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(ETagACL), ETagACL.Add)]
        public RedirectToRouteResult Add_POST(ETag eTag)
        {
            _eTagAdminService.Add(eTag);
            return RedirectToAction("Edit", new { id = eTag.Id });
        }

        [HttpGet]
        [MrCMSACLRule(typeof(ETagACL), ETagACL.Edit)]
        public ViewResult Edit(ETag eTag)
        {
            return View(eTag);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(ETagACL), ETagACL.Edit)]
        public RedirectToRouteResult Edit_POST(ETag eTag)
        {
            _eTagAdminService.Update(eTag);

            return RedirectToAction("Edit", new { id = eTag.Id });
        }

        [HttpGet]
        [MrCMSACLRule(typeof(ETagACL), ETagACL.Delete)]
        public PartialViewResult Delete(ETag eTag)
        {
            return PartialView(eTag);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(DiscountACL), DiscountACL.Delete)]
        public RedirectToRouteResult Delete_POST(ETag eTag)
        {
            _eTagAdminService.Delete(eTag);

            return RedirectToAction("Index");
        }
    }
}