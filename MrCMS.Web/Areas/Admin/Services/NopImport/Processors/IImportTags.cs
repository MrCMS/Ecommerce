namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportTags
    {
        string ProcessTags(INopCommerceProductReader nopCommerceProductReader, string connectionString, NopImportContext nopImportContext);
    }
}