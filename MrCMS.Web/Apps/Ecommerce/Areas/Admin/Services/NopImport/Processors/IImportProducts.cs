namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportProducts
    {
        string ProcessProducts(NopCommerceDataReader importParams, NopImportContext nopImportContext);
    }
}