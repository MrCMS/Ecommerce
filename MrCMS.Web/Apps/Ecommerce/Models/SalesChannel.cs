using System.ComponentModel;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public enum SalesChannel
    {
        [Description("Mr CMS")]
        MrCMS,
        [Description("Amazon")]
        Amazon,
        [Description("eBay")]
        EBay
    }
}