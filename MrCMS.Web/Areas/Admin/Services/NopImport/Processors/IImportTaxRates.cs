namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportTaxRates
    {
        string ProcessTaxRates(INopCommerceProductReader nopCommerceProductReader, string connectionString, NopImportContext nopImportContext);
    }
}