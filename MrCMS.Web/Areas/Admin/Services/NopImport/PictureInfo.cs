namespace MrCMS.Web.Areas.Admin.Services.NopImport
{
    public struct PictureInfo
    {
        public PictureInfo(PictureLocation pictureLocation, string locationData) : this()
        {
            PictureLocation = pictureLocation;
            LocationData = locationData;
        }

        public PictureLocation PictureLocation { get; set; }
        public string LocationData { get; set; }
    }
}