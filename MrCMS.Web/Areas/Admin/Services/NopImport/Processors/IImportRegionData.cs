namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportRegionData
    {
        string ProcessRegions(INopCommerceProductReader nopCommerceProductReader, string connectionString, NopImportContext nopImportContext);
    }
}