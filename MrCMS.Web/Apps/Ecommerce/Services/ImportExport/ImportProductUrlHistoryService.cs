using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public class ImportProductUrlHistoryService : IImportProductUrlHistoryService
    {
        private readonly ISession _session;

        public ImportProductUrlHistoryService(ISession session)
        {
            _session = session;
        }
        public void ImportUrlHistory(ProductImportDataTransferObject productDto, Product product)
        {
            List<string> urlsToAdd =
                 productDto.UrlHistory.Where(
                     s =>
                         !product.Urls.Select(history => history.UrlSegment)
                             .Contains(s, StringComparer.InvariantCultureIgnoreCase)).ToList();
            //List<UrlHistory> urlsToRemove =
            //    product.Urls.Where(
            //        history =>
            //            !productDto.UrlHistory.Contains(history.UrlSegment, StringComparer.InvariantCultureIgnoreCase))
            //        .ToList();
            foreach (string item in urlsToAdd)
            {
                UrlHistory history =
                    _session.Query<UrlHistory>().FirstOrDefault(urlHistory => urlHistory.UrlSegment == item);
                bool isNew = history == null;
                if (isNew)
                {
                    history = new UrlHistory { UrlSegment = item, Webpage = product };
                    _session.Transact(session => session.Save(history));
                }
                else
                    history.Webpage = product;
                if (!product.Urls.Contains(history))
                    product.Urls.Add(history);
                _session.Transact(session => session.Update(history));
            }

            //foreach (UrlHistory history in urlsToRemove)
            //{
            //    product.Urls.Remove(history);
            //    history.Webpage = null;
            //    UrlHistory closureHistory = history;
            //    _session.Transact(session => session.Update(closureHistory));
            //}
        }
    }
}