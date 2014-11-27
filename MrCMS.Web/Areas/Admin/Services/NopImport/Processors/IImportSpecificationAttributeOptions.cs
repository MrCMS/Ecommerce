namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportSpecificationAttributeOptions
    {
        string ProcessSpecificationAttributeOptions(INopCommerceProductReader nopCommerceProductReader, string connectionString, NopImportContext nopImportContext);
    }
}