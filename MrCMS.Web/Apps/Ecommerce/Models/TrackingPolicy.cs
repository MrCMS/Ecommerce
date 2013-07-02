using System.ComponentModel;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public enum TrackingPolicy
    {
        [Description("Track")]
        Track,
        [Description("Don't Track")]
        DontTrack
    }
}