namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportSpecifications
    {
        string ProcessSpecifications(NopCommerceDataReader importParams, NopImportContext nopImportContext);
    }
}