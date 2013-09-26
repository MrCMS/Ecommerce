using System.ComponentModel;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public enum AmazonLogStatus
    {
        [Description("Initiation")]
        Initiation,
        [Description("Stage")]
        Stage,
        [Description("Operation successfully completed")]
        Completion,
        [Description("Error happened.")]
        Error,
        [Description("New item inserted")]
        Insert,
        [Description("Item successfully updated")]
        Update,
        [Description("Item deleted")]
        Delete
    }
}