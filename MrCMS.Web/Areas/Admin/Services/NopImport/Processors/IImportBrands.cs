namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportBrands
    {
        string ProcessBrands(NopCommerceDataReader dataReader, NopImportContext nopImportContext);
    }
}