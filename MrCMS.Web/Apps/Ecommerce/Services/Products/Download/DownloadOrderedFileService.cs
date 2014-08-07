using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Elmah;
using MrCMS.Logging;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Products.Download.Rules;
using System.Web.Mvc;
using System.Linq;
using MrCMS.Web.Areas.Admin.Services;
using NHibernate;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products.Download
{
    public class EcommerceDownloadResult : ActionResult
    {
        private readonly IFileSystem _fileSystem;
        private readonly OrderLine _orderLine;

        public EcommerceDownloadResult(IFileSystem fileSystem, OrderLine orderLine)
        {
            _fileSystem = fileSystem;
            _orderLine = orderLine;
        }

        public OrderLine OrderLine
        {
            get { return _orderLine; }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Buffer = false;
            context.HttpContext.Response.AddHeader("Content-Disposition", "attachment; filename=" + OrderLine.DownloadFileName);
            _fileSystem.WriteToStream(OrderLine.DownloadFileUrl, context.HttpContext.Response.OutputStream);
        }
    }
    public class DownloadOrderedFileService : IDownloadOrderedFileService
    {
        private readonly ISession _session;
        private readonly IEnumerable<IDownloadOrderedFileValidationRule> _rules;
        private readonly IFileSystem _fileSystem;
        private readonly ILogAdminService _logService;

        public DownloadOrderedFileService(ISession session, IEnumerable<IDownloadOrderedFileValidationRule> rules, IFileSystem fileSystem, ILogAdminService logService)
        {
            _session = session;
            _rules = rules;
            _fileSystem = fileSystem;
            _logService = logService;
        }

        public ActionResult WriteDownloadToResponse(HttpResponseBase response, Order order, OrderLine orderLine)
        {
            if (order == null || orderLine == null)
                return new ContentResult { Content = "Error", ContentType = "text/plain" };

            var errors = _rules.SelectMany(rule => rule.GetErrors(order, orderLine));
            if (errors.Any())
                return new ContentResult { Content = "Sorry the file you requested to be downloaded is unavailable. Either the order is not paid or the file has been downloaded a maximum amount of times. Contact the store owner if you believe this to be incorrect.", ContentType = "text/plain" };

            if (!_fileSystem.Exists(orderLine.DownloadFileUrl))
                return new ContentResult { Content = "File no longer exists.", ContentType = "text/plain" };

            EcommerceDownloadResult writeDownloadToResponse;
            try
            {
                writeDownloadToResponse = new EcommerceDownloadResult(_fileSystem, orderLine);
            }
            catch (Exception exception)
            {
                _logService.Insert(new Log
                    {
                        Error = new Error(exception),
                        Message = "Error downloading file"
                    });
                return new ContentResult { Content = "An error occoured, please contact the store owner.", ContentType = "text/plain" };
            }

            orderLine.NumberOfDownloads++;
            _session.Transact(session => session.Update(orderLine));
            return writeDownloadToResponse;
        }
    }
}