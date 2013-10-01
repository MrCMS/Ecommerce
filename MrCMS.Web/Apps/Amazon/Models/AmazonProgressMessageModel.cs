using System;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public class AmazonProgressMessageModel
    {
        public AmazonProgressMessageModel()
        {
            Stage = "Start";
            Message = String.Empty;
            Created = CurrentRequestData.Now;
        }

        public string Stage { get; set; }
        public string Message { get; set; }
        public DateTime? Created { get; set; }
    }
}