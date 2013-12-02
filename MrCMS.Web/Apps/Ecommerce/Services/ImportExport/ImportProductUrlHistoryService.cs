using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public class ImportProductUrlHistoryService : IImportProductUrlHistoryService
    {
        private readonly ISession _session;
        private HashSet<UrlHistory> _urlHistories;

        public ImportProductUrlHistoryService(ISession session)
        {
            _session = session;
        }

        public IImportProductUrlHistoryService Initialize()
        {
            _urlHistories = new HashSet<UrlHistory>(_session.QueryOver<UrlHistory>().List());
            return this;
        }

        public IEnumerable<UrlHistory> ImportUrlHistory(ProductImportDataTransferObject item, Product product)
        {
            foreach (var urlHistoryItem in item.UrlHistory)
            {
                var urlHistory = _urlHistories.FirstOrDefault(history => history.UrlSegment == urlHistoryItem.Trim());
                if (urlHistory == null)
                {
                    urlHistory = new UrlHistory
                                     {
                                         UrlSegment = urlHistoryItem.Trim(),
                                         Webpage = product
                                     };
                    product.Urls.Add(urlHistory);
                }
            }
            return product.Urls;
        }
    }
}