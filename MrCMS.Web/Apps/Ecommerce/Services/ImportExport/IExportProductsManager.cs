namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public interface IExportProductsManager
    {
        byte[] ExportProductsToExcel();
    }
}