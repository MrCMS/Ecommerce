using System.ComponentModel;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public enum AmazonLogStatus
    {
        [Description("Stage")]
        Stage,
        [Description("Error")]
        Error,
        [Description("Completion")]
        Completion
    }
}