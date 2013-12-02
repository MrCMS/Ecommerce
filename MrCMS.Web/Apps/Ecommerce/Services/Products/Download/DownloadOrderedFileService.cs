using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Products.Download.Rules;
using System.Web.Mvc;
using System.Linq;
using NHibernate;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products.Download
{
    public class DownloadOrderedFileService : IDownloadOrderedFileService
    {
        private readonly ISession _session;
        private readonly IEnumerable<IDownloadOrderedFileValidationRule> _rules;
        private readonly IFileSystem _fileSystem;

        public DownloadOrderedFileService(ISession session, IEnumerable<IDownloadOrderedFileValidationRule> rules, IFileSystem fileSystem)
        {
            _session = session;
            _rules = rules;
            _fileSystem = fileSystem;
        }

        public void WriteDownloadToResponse(HttpResponseBase response, Order order, OrderLine orderLine)
        {
            if (order == null || orderLine == null)
                return;

            var errors = _rules.SelectMany(rule => rule.GetErrors(order, orderLine));
            if (errors.Any())
                return;

            if (!_fileSystem.Exists(orderLine.DownloadFileUrl))
                return;

            try
            {
                response.Buffer = false;
                response.AddHeader("Content-Disposition", "attachment; filename=" + orderLine.DownloadFileName);
                _fileSystem.WriteToStream(orderLine.DownloadFileUrl, response.OutputStream);
            }
            catch
            {
                return;
            }

            orderLine.NumberOfDownloads++;
            _session.Transact(session => session.Update(orderLine));
        }
    }
}