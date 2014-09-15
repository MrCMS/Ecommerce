using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class GiftMessageController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IGiftMessageUIService _giftMessageUIService;

        public GiftMessageController(IGiftMessageUIService giftMessageUIService)
        {
            _giftMessageUIService = giftMessageUIService;
        }

        [HttpPost]
        public ActionResult Save(string message)
        {
            _giftMessageUIService.Save(message);
            return Json(true);
        }
    }
}