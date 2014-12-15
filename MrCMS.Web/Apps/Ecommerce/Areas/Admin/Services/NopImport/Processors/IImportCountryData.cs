namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportCountryData
    {
        string ProcessCountries(NopCommerceDataReader importParams, NopImportContext nopImportContext);
    }
}