namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportTaxRates
    {
        string ProcessTaxRates(NopCommerceDataReader importParams, NopImportContext nopImportContext);
    }
}