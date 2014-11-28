namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportRegionData
    {
        string ProcessRegions(NopCommerceDataReader importParams, NopImportContext nopImportContext);
    }
}