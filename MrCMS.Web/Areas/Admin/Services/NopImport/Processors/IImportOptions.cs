namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportOptions
    {
        string ProcessOptions(NopCommerceDataReader importParams, NopImportContext nopImportContext);
    }
}