using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services.GoogleBase
{
    public class GoogleBaseCategory
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public static class GoogleBaseTaxonomyData
    {
        private static IEnumerable<string> RawCategories { get; set; }

        public static IEnumerable<GoogleBaseCategory> GetCategories()
        {
            SetTaxonomyData(MrCMSApplication.Get<GoogleBaseSettings>().GoogleBaseTaxonomyFeedUrl);
            return
                RawCategories.Select(
                    googleBaseCategory =>
                    new GoogleBaseCategory() {Name = googleBaseCategory, Id=googleBaseCategory}).ToList();
        }

        private static void SetTaxonomyData(string url)
        {
            if (RawCategories != null || String.IsNullOrWhiteSpace(url)) return;

            var result = DownloadRawTaxonomyData(url);

            if (String.IsNullOrWhiteSpace(result)) return;

            var rows = result.Split(new[] {"\n", "\r"}, StringSplitOptions.RemoveEmptyEntries);
            rows.ToList().RemoveAt(0);
            RawCategories = rows;
        }

        private static string DownloadRawTaxonomyData(string url)
        {
            try
            {
                var client = new WebClient();
                return client.DownloadString(url);
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

    }
}