using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ContactUsController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IUniquePageService _uniquePageService;
        
        public ContactUsController(IUniquePageService uniquePageService)
        {
            _uniquePageService = uniquePageService;
        }

        public ActionResult Show(ContactUs page)
        {
            return View(page);
        }

        public JsonResult GenerateMap()
        {
            var page = _uniquePageService.GetUniquePage<ContactUs>();
            if (page != null)
            {
                var data = new MapViewModel
                {
                    Latitude = page.Latitude,
                    Longitude = page.Longitude,
                    MapPinIcon = page.PinImage
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
    }

    public class MapViewModel
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string MapPinIcon { get; set; }
    }
}