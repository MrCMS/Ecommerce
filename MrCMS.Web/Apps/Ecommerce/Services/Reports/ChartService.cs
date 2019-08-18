using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Reports
{
    public class ChartService : IChartService
    {
        public void SetBarChartLabelsAndData(ref ChartModel model,
            Dictionary<string, IList<KeyValuePair<string, decimal>>> items)
        {
            model.Labels = new List<string>();
            model.MultipleData = new Dictionary<string, List<decimal>>();

            foreach (string label in items.Values.SelectMany(item => item.Select(x => x.Key).Distinct()))
            {
                if (model.Labels.All(x => x != label))
                    model.Labels.Add(label);
            }

            foreach (string key in items.Keys)
            {
                IList<KeyValuePair<string, decimal>> item = items[key];
                List<decimal> data =
                    model.Labels.Select(
                        label => item.Any(x => x.Key == label) ? item.SingleOrDefault(x => x.Key == label).Value : 0)
                        .ToList();
                model.MultipleData.Add(key, data.Any(x => x != 0) ? data : new List<decimal>());
            }
        }

        public void SetLineChartData(ref ChartModel model,
            Dictionary<string, IList<KeyValuePair<DateTime, decimal>>> items)
        {
            foreach (string key in items.Keys)
            {
                IList<KeyValuePair<DateTime, decimal>> item = items[key];
                var data = new List<decimal>();
                TimeSpan ts = model.To - model.From;
                DateTime oldDate = DateTime.Parse(model.From.Date.ToString());
                DateTime currentDate = oldDate;

                if (ts.Days == 1)
                {
                    for (int i = 0; i < 24; i++)
                    {
                        data.Add(item.Where(x => x.Key.Hour == i + 1).Sum(x => x.Value));
                    }
                }
                else
                {
                    for (int i = 0; i < ts.Days; i++)
                    {
                        data.Add(item.Where(x => x.Key == currentDate.AddDays(i)).Sum(x => x.Value));
                    }
                }

                model.MultipleData.Add(key, data.Any(x => x != 0) ? data : new List<decimal>());
            }
        }

        public void SetLineChartLabels(ref ChartModel model)
        {
            model.Labels = new List<string>();
            TimeSpan ts = model.To - model.From;
            DateTime oldDate = DateTime.Parse(model.From.Date.ToString());
            DateTime currentDate = oldDate;

            if (ts.Days == 1)
            {
                for (int i = 0; i < 24; i++)
                {
                    oldDate = currentDate;
                    currentDate = oldDate.AddHours(1);
                    model.Labels.Add(oldDate.Hour.ToString());
                }
            }
            else
            {
                for (int i = 0; i < ts.Days; i++)
                {
                    oldDate = currentDate;
                    currentDate = oldDate.AddDays(1);
                    model.Labels.Add(oldDate.ToString("dd/MM"));
                }
            }
        }
    }
}