using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public interface IImportProductUrlHistoryService
    {
        IEnumerable<UrlHistory> ImportUrlHistory(ProductImportDataTransferObject item, Product product);
    }
}