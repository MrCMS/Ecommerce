namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportBrands
    {
        string ProcessBrands(INopCommerceProductReader nopCommerceProductReader, string connectionString, NopImportContext nopImportContext);
    }
}