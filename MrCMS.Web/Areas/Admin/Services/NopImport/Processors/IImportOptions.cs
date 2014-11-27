namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportOptions
    {
        string ProcessOptions(INopCommerceProductReader nopCommerceProductReader, string connectionString, NopImportContext nopImportContext);
    }
}