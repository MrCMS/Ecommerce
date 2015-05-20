using System.Web.Mvc;
using MrCMS.Services.Resources;
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
        private readonly IStringResourceProvider _stringResourceProvider;

        public ETagController(IETagAdminService eTagAdminService, IStringResourceProvider  stringResourceProvider)
        {
            _eTagAdminService = eTagAdminService;
            _stringResourceProvider = stringResourceProvider;
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

        public ActionResult ValidateNameIsAllowed(string name, int? id)
        {
            return _eTagAdminService.NameIsValidForETag(name, id)
                ? Json(_stringResourceProvider.GetValue("Ecommerce - Admin - ETag name validation","Please choose a different name as this one is already used."), JsonRequestBehavior.AllowGet)
                : Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}