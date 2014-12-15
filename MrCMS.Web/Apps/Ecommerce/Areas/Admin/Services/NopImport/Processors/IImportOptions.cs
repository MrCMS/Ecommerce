namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportOptions
    {
        string ProcessOptions(NopCommerceDataReader importParams, NopImportContext nopImportContext);
    }
}