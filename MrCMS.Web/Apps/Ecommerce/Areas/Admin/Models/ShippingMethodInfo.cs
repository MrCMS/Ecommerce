namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class ShippingMethodInfo
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }

        public string ConfigureUrl { get; set; }
        public bool Configurable { get { return !string.IsNullOrWhiteSpace(ConfigureUrl); } }

        public bool Enabled { get; set; }
        public string Type { get; set; }
    }
}