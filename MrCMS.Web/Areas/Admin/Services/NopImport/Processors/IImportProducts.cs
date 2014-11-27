namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportProducts
    {
        string ProcessProducts(INopCommerceProductReader nopCommerceProductReader, string connectionString, NopImportContext nopImportContext);
    }
}