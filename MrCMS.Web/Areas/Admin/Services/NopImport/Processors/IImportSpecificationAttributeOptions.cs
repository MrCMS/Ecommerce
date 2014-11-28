namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportSpecificationAttributeOptions
    {
        string ProcessSpecificationAttributeOptions(NopCommerceDataReader importParams, NopImportContext nopImportContext);
    }
}