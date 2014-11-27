namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportSpecifications
    {
        string ProcessSpecifications(INopCommerceProductReader nopCommerceProductReader, string connectionString, NopImportContext nopImportContext);
    }
}