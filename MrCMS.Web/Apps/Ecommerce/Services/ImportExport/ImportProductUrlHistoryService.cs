using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public class ImportProductUrlHistoryService : IImportProductUrlHistoryService
    {
        private readonly IUrlHistoryService _urlHistoryService;

        public ImportProductUrlHistoryService(IUrlHistoryService urlHistoryService)
        {
            _urlHistoryService = urlHistoryService;
        }

        public IEnumerable<UrlHistory> ImportUrlHistory(ProductImportDataTransferObject item, Product product)
        {
            foreach (var urlHistoryItem in item.UrlHistory)
            {
                var urlHistory = _urlHistoryService.GetByUrlSegment(urlHistoryItem.Trim());
                if (urlHistory == null)
                {
                    urlHistory = new UrlHistory()
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