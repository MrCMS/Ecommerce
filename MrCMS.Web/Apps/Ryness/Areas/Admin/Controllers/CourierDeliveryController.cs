using System;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Web.Apps.Ryness.Models;
using MrCMS.Web.Apps.Ryness.Services;
using MrCMS.Website.Controllers;
using NHibernate;

namespace MrCMS.Web.Apps.Ryness.Areas.Admin.Controllers
{
    public class CourierDeliveryController : MrCMSAppAdminController<RynessApp>
    {
        private readonly ICourierDeliveryService _courierDeliveryService;

        public CourierDeliveryController(ICourierDeliveryService courierDeliveryService)
        {
            _courierDeliveryService = courierDeliveryService;
        }

        [HttpGet]
        public ViewResult Index(CourierDeliverySearchModel model)
        {
            if (model.DateFrom.HasValue && model.DateTo.HasValue)
            {
                try
                {
                    model.Results = _courierDeliveryService.GetCourierDeliveryOrderByDates((DateTime) model.DateFrom, (DateTime) model.DateTo);
                }
                catch (Exception)
                {
                }
            }
            else
            {
                model.DateFrom = DateTime.Today;
                model.DateTo = DateTime.Today.AddDays(1);
                model.Results = _courierDeliveryService.GetCourierDeliveryOrderByDates((DateTime)model.DateFrom, (DateTime)model.DateTo);
            }
            return View("Index", model);
        }
    }
}
