namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportSpecificationAttributeOptions
    {
        string ProcessSpecificationAttributeOptions(NopCommerceDataReader importParams, NopImportContext nopImportContext);
    }
}