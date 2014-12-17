using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class TermsAndConditionsController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly ITermsAndConditionsUIService _termsAndConditionsUIService;

        public TermsAndConditionsController(ITermsAndConditionsUIService termsAndConditionsUIService)
        {
            _termsAndConditionsUIService = termsAndConditionsUIService;
        }

        public JsonResult Set(bool accept)
        {
            _termsAndConditionsUIService.SetAcceptance(accept);
            return Json(true);
        }
    }
}