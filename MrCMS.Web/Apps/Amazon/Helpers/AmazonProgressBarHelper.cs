using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Helpers
{
    public static class AmazonProgressBarHelper
    {
        private static readonly IDictionary<Guid, AmazonProgressModel> ProgressBars = new Dictionary<Guid, AmazonProgressModel>();

        public static AmazonProgressModel GetProgressBar(Guid? id)
        {
            var currentProgress = new AmazonProgressModel();
            if (id.HasValue)
                currentProgress = ProgressBars.Keys.Contains(id.Value) ? ProgressBars[id.Value] : new AmazonProgressModel();
            return currentProgress;
        }

        private static void SetProgressBar(Guid id, AmazonProgressModel progressModel)
        {
            if (ProgressBars.Keys.Contains(id))
                ProgressBars[id] = progressModel;
        }

        public static void CleanProgressBars(Guid id)
        {
            if (ProgressBars.Keys.Contains(id))
                ProgressBars.Remove(id);

            var progressBars = ProgressBars
          .Where(progressBar => progressBar.Value.IsComplete || (CurrentRequestData.Now - progressBar.Value.StartTime).Minutes > 60)
          .Select(progressBar => progressBar.Key)
          .ToList();

            foreach (var progressBar in progressBars)
                ProgressBars.Remove(progressBar);
        }

        public static void Update(Guid id,string status,string message, int? totalRecords, int? processedRecords)
        {
            if(!ProgressBars.Keys.Contains(id))
                ProgressBars.Add(id, new AmazonProgressModel(){TaskId = id});

            var progressModel = GetProgressBar(id);

            if (!String.IsNullOrWhiteSpace(message) || !String.IsNullOrWhiteSpace(status))
                progressModel.Messages.Add(new AmazonProgressMessageModel()
                    {
                        Message = message,
                        Stage = status,
                        Created = CurrentRequestData.Now
                    });
            if (totalRecords.HasValue)
                progressModel.TotalActions = totalRecords.Value;
            if (processedRecords.HasValue)
                progressModel.ProcessedActions = processedRecords.Value;

            SetProgressBar(id, progressModel);
        }

        public static object GetStatus(Guid? id)
        {
            var currentProgress = new AmazonProgressModel();
            if (id.HasValue)
                currentProgress = ProgressBars.Keys.Contains(id.Value) ? ProgressBars[id.Value] : new AmazonProgressModel();
            return new
            {
               currentProgress.PercentComplete,
               Status = currentProgress.Messages.Any()?currentProgress.Messages.Last().Stage:String.Empty
            };
        }
    }
}