using System;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public class AmazonProgressModel
    {
        public AmazonProgressModel()
        {
            Statuses=new List<string>();
            CurrentStatus = string.Empty;
            TotalActions = 100;
            ProcessedActions = 0;
        }

        public List<string> Statuses { get; set; }
        public string CurrentStatus { get; set; }
        public int TotalActions { get; set; }
        public int ProcessedActions { get; set; }
        public decimal PercentComplete 
        {
            get
            {
                return Math.Round(((decimal)ProcessedActions / TotalActions) * 100, 0);
            }
        }
    }
}