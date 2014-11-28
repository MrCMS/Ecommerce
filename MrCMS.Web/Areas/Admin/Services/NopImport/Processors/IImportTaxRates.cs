namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportTaxRates
    {
        string ProcessTaxRates(NopCommerceDataReader importParams, NopImportContext nopImportContext);
    }
}