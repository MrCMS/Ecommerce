using System;
using System.IO;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models
{
    public class PictureData
    {
        public int Id { get; set; }
        public Func<Stream> GetData { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}