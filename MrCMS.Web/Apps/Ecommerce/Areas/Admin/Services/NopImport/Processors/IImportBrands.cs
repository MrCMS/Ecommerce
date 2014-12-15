namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportBrands
    {
        string ProcessBrands(NopCommerceDataReader dataReader, NopImportContext nopImportContext);
    }
}