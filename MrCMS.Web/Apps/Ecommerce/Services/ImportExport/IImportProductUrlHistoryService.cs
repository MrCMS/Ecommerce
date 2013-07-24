using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public interface IImportProductUrlHistoryService
    {
        void ImportUrlHistory(ProductImportDataTransferObject item, Product product);
    }
}