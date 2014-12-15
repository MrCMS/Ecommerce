namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportTags
    {
        string ProcessTags(NopCommerceDataReader importParams, NopImportContext nopImportContext);
    }
}