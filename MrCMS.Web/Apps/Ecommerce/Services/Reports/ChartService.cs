using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Reports
{
    public interface IChartService
    {
        void SetLineChartLabels(ref ChartModel model);
        void SetLineChartData(ref ChartModel model, IList<KeyValuePair<DateTime, decimal>> items);
        void SetLineChartData(ref ChartModel model, IEnumerable<IList<KeyValuePair<DateTime, decimal>>> items);
        void SetPieChartLabelsAndData(ref ChartModel model, IEnumerable<KeyValuePair<string, decimal>> items);
    }

    public class ChartService : IChartService
    {
        public void SetLineChartData(ref ChartModel model, IList<KeyValuePair<DateTime, decimal>> items)
        {
            var data = new List<decimal>();
            var ts = model.To - model.From;
            var oldDate = DateTime.Parse(model.From.Date.ToString());
            var currentDate = oldDate;

            for (var i = 0; i < ts.Days; i++)
            {
                oldDate = currentDate;
                currentDate = oldDate.AddDays(1);
                data.Add(i == 0
                             ? items.Where(x => x.Key.Date == currentDate.Date).Sum(x => x.Value)
                             : items.Where(x => oldDate.Date <= x.Key && x.Key < currentDate.Date)
                                    .Sum(x => x.Value));
            }

            model.LineChartData.Add(model.LineChartData.Count.ToString(), new List<decimal>());
        }

        public void SetLineChartData(ref ChartModel model, IEnumerable<IList<KeyValuePair<DateTime, decimal>>> items)
        {
            foreach (var item in items)
            {
                var data = new List<decimal>();
                var ts = model.To - model.From;
                var oldDate = DateTime.Parse(model.From.Date.ToString());
                var currentDate = oldDate;

                if (model.From.Month == model.To.Month || ts.Days < 31)
                {
                    for (var i = 0; i < ts.Days; i++)
                    {
                        oldDate = currentDate;
                        currentDate = oldDate.AddDays(1);
                        data.Add(i == 0
                                     ? item.Where(x => x.Key.Date == currentDate.Date).Sum(x => x.Value)
                                     : item.Where(x => oldDate.Date <= x.Key && x.Key < currentDate.Date)
                                            .Sum(x => x.Value));
                    }
                }
                else
                {
                    while (oldDate.Month <= currentDate.Month)
                    {
                        oldDate = currentDate;
                        data.Add(item.Where(x => x.Key.Month == currentDate.Month).Sum(x => x.Value));
                        currentDate = oldDate.AddMonths(1);
                    }
                }

                model.LineChartData.Add(model.LineChartData.Count.ToString(), data);
            }
        }

        public void SetLineChartLabels(ref ChartModel model)
        {
            model.LineChartLabels = new List<string>();
            var ts = model.To - model.From;
            var oldDate = DateTime.Parse(model.From.Date.ToString());
            var currentDate = oldDate;

            if (model.From.Month == model.To.Month || ts.Days<31)
            {
                for (var i = 0; i < ts.Days; i++)
                {
                    oldDate = currentDate;
                    currentDate = oldDate.AddDays(1);
                    model.LineChartLabels.Add(oldDate.ToString("dd/MM"));
                }
            }
            else
            {
                while (oldDate.Month <= currentDate.Month)
                {
                    oldDate = currentDate;
                    currentDate = oldDate.AddMonths(1);
                    model.LineChartLabels.Add(oldDate.Month.GetMonth());
                }
            }
        }

        public void SetPieChartLabelsAndData(ref ChartModel model, IEnumerable<KeyValuePair<string, decimal>> items)
        {
            model.PieChartData=items.ToDictionary(t => t.Key, t => t.Value);
        }
    }
}