namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportPictureData
    {
        string ImportPictures(NopCommerceDataReader dataReader, NopImportContext nopImportContext);
    }
}