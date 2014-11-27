namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportCountryData
    {
        string ProcessCountries(INopCommerceProductReader nopCommerceProductReader, string connectionString, NopImportContext nopImportContext);
    }
}