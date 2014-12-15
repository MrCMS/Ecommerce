namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportUsers
    {
        string ProcessUsers(NopCommerceDataReader dataReader, NopImportContext nopImportContext);
    }
}