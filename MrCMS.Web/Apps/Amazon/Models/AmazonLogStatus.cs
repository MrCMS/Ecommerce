using System.ComponentModel;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public enum AmazonLogStatus
    {
        [Description("Initiation")]
        Initiation,
        [Description("Stage")]
        Stage,
        [Description("Completion")]
        Completion,
        [Description("Error")]
        Error,
        [Description("Insert")]
        Insert,
        [Description("Update")]
        Update,
        [Description("Delete")]
        Delete
    }
}