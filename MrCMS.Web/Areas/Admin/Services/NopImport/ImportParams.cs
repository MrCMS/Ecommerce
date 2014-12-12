using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Areas.Admin.Services.NopImport
{
    public class ImportParams
    {
        [Required]
        public string ImporterType { get; set; }

        [Required]
        public string ConnectionString { get; set; }

        public PictureLocation PictureLocation { get; set; }
        public string LocationData { get; set; }

        public PictureInfo PictureInfo
        {
            get { return new PictureInfo(PictureLocation, LocationData); }
        }
    }
}