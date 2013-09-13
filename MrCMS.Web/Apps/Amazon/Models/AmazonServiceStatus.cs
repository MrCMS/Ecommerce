using System.ComponentModel;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public enum AmazonServiceStatus
    {
        [Description("Amazon Api Order service is operating normally.")]
        GREEN = 0,
        [Description("Amazon Api Order service is operating normally. Additional information is provided.")]
        GREEN_I = 1,
        [Description("Amazon Api Order service is experiencing higher than normal error rates or is operating with degraded performance. Additional information may be provided.")]
        YELLOW = 2,
        [Description("The service is unavailable or experiencing extremely high error rates.")]
        RED = 3
    }
}