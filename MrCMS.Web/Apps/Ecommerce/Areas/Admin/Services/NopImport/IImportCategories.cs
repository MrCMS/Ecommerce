namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport
{
    public interface IImportCategories
    {
        string ProcessCategories(NopCommerceDataReader dataReader, NopImportContext nopImportContext);
    }
}