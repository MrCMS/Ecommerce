namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportProducts
    {
        string ProcessProducts(NopCommerceDataReader importParams, NopImportContext nopImportContext);
    }
}