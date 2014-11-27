namespace MrCMS.Web.Areas.Admin.Services.NopImport
{
    public interface IImportCategories
    {
        string ProcessCategories(INopCommerceProductReader nopCommerceProductReader, string connectionString, NopImportContext nopImportContext);
    }
}