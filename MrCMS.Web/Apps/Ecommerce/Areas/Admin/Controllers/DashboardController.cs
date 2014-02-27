using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Reports;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class DashboardController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly ICurrentSiteLocator _currentSiteLocator;
        private readonly IUserService _userServices;
        private readonly ISession _session;
        private readonly IReportService _reportService;

        public DashboardController(ICurrentSiteLocator currentSiteLocator, IUserService userServices, ISession session, IReportService reportService)
        {
            _currentSiteLocator = currentSiteLocator;
            _userServices = userServices;
            _session = session;
            _reportService = reportService;
        }

        [HttpGet]
        public ActionResult Revenue()
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult RevenueToday()
        {
            return Json(_reportService.SalesTodayGroupedByHour());
        }

        [HttpPost]
        public JsonResult RevenueThisWeek()
        {
            return Json(_reportService.SalesLastWeekGroupedByDay());
        }

        [HttpGet]
        public ActionResult UserAndPageStats()
        {
            //todo this
            var model = new Dashboard
                {
                    ActiveUsers = 10,
                    Stats = GetPageStats(),
                    NoneActiveUsers = 10
                };
            return PartialView(model);
        }

        private IList<WebpageStats> GetPageStats()
        {
            WebpageStats countAlias = null;
            Webpage webpageAlias = null;
            var currentSite = _currentSiteLocator.GetCurrentSite();
            var list = _session.QueryOver(() => webpageAlias)
                               .Where(x => x.Site == currentSite)
                               .SelectList(
                                   builder =>
                                   builder.SelectGroup(() => webpageAlias.DocumentType)
                                          .WithAlias(() => countAlias.DocumentType)
                                          .SelectCount(() => webpageAlias.Id)
                                          .WithAlias(() => countAlias.NumberOfPages)
                                          .SelectSubQuery(
                                              QueryOver.Of<Webpage>().Where(webpage => webpage.Site == currentSite
                                                                                       &&
                                                                                       webpage.DocumentType ==
                                                                                       webpageAlias.DocumentType &&
                                                                                       (webpage.PublishOn == null
                                                                                        ||
                                                                                        webpage.PublishOn >
                                                                                        CurrentRequestData.Now))
                                                       .ToRowCountQuery())
                                          .WithAlias(() => countAlias.NumberOfUnPublishedPages))
                               .TransformUsing(Transformers.AliasToBean<WebpageStats>())
                               .List<WebpageStats>();
            return list;
        }

        //[HttpGet]
        //public string TestDigitalDownload(Entities.Orders.Order order)
        //{
        //    if (order == null) return string.Empty;

        //    var productVariant = order.OrderLines.First().ProductVariant;

        //    if (productVariant == null) return string.Empty;

        //    var url = string.Format("digital-download/{0}/{1}/{2}", order.Guid, productVariant.Id, string.Empty);

        //    return url;
        //}
    }
}