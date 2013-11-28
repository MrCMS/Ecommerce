using System.Collections.Generic;
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

        public DownloadOrderedFileService(ISession session, IEnumerable<IDownloadOrderedFileValidationRule> rules)
        {
            _session = session;
            _rules = rules;
        }

        public FilePathResult GetDownload(Order order, OrderLine orderLine)
        {
            if (order == null || orderLine == null)
                return null;

            var errors = _rules.SelectMany(rule => rule.GetErrors(order, orderLine));
            if (errors.Any())
                return null;

            orderLine.NumberOfDownloads++;
            _session.Transact(session => session.Update(orderLine));

            return new FilePathResult(orderLine.DownloadFileUrl, orderLine.DownloadFileContentType)
                       {
                           FileDownloadName = orderLine.DownloadFileName
                       };
        }
    }
}