namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportAddresses
    {
        string ProcessAddresses(NopCommerceDataReader dataReader, NopImportContext nopImportContext);
    }
}