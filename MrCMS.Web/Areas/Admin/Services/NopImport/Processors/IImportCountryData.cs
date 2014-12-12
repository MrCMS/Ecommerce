namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportCountryData
    {
        string ProcessCountries(NopCommerceDataReader importParams, NopImportContext nopImportContext);
    }
}