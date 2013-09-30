using System;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public class AmazonProgressMessageModel
    {
        public AmazonProgressMessageModel()
        {
            Stage = "Start";
            Message = String.Empty;
            Created = DateTime.UtcNow;
        }

        public string Stage { get; set; }
        public string Message { get; set; }
        public DateTime? Created { get; set; }
    }
}