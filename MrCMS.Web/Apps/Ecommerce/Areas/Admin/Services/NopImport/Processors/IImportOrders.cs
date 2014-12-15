namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportOrders
    {
        string ProcessOrders(NopCommerceDataReader dataReader, NopImportContext nopImportContext);
    }
}