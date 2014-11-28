namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportTags
    {
        string ProcessTags(NopCommerceDataReader importParams, NopImportContext nopImportContext);
    }
}