namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models
{
    public class CategoryData
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public string Abstract { get; set; }

        public bool Published { get; set; }

        public int PictureId { get; set; }
    }
}