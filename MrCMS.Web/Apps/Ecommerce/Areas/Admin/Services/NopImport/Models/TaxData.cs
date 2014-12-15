namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models
{
    public class TaxData
    {
        public string Name { get; set; }
        public decimal Rate { get; set; }
        public int Id{ get; set; }

        public int? RegionId { get; set; }
    }
}