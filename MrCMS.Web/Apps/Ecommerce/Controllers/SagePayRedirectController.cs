using System;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class SagePayRedirectController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IUniquePageService _uniquePageService;

        public SagePayRedirectController(IUniquePageService uniquePageService)
        {
            _uniquePageService = uniquePageService;
        }

        public ActionResult Failed(string vendorTxCode)
        {
            return View("Failed");
        }

        [HttpPost]
        public RedirectResult FailedPost()
        {
            return _uniquePageService.RedirectTo<PaymentDetails>();
        }

        [ForceImmediateLuceneUpdate]
        public ActionResult Success(string vendorTxCode)
        {
            return View((object)vendorTxCode);
        }

        [HttpPost]
        public RedirectResult SuccessPost(Guid id)
        {
            return _uniquePageService.RedirectTo<OrderPlaced>(new { id });
        }
    }
}