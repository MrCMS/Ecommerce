using System.Web.Mvc;
using MrCMS.Web.Apps.Ryness.Entities;
using MrCMS.Web.Apps.Ryness.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ryness.Areas.Admin.Controllers
{
    public class TestimonialController : MrCMSAppAdminController<RynessApp>
    {
        private readonly ITestimonialService _testimonialService;

        public TestimonialController(ITestimonialService testimonialService)
        {
            _testimonialService = testimonialService;
        }

        public ViewResult Index()
        {
            var testimonials = _testimonialService.GetAll();
            return View(testimonials);
        }

        [HttpGet]
        public PartialViewResult Add()
        {
            return PartialView();
        }

        [HttpPost]
        public RedirectToRouteResult Add(Testimonial testimonial)
        {
            _testimonialService.Add(testimonial);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Edit")]
        public PartialViewResult Edit_GET(Testimonial testimonial)
        {
            return PartialView(testimonial);
        }

        [HttpPost]
        public RedirectToRouteResult Edit(Testimonial testimonial)
        {
            _testimonialService.Update(testimonial);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Delete")]
        public PartialViewResult Delete_GET(Testimonial testimonial)
        {
            return PartialView(testimonial);
        }

        [HttpPost]
        public RedirectToRouteResult Delete(Testimonial testimonial)
        {
            _testimonialService.Delete(testimonial);
            return RedirectToAction("Index");
        }
    }
}