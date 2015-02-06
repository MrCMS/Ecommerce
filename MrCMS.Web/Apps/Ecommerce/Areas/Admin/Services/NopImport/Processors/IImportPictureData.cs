namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportPictureData
    {
        string ImportPictures(NopCommerceDataReader dataReader, NopImportContext nopImportContext);
    }
}