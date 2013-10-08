using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Serialization;
using MarketplaceWebServiceFeedsClasses;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Helpers
{
    public static class AmazonAppHelper
    {
        #region Misc

        public static T GetEnumByValue<T>(this string value) where T : struct
        {
            return Enum.GetValues(typeof(T)).Cast<T>().SingleOrDefault(x => x.ToString() == value);
        }
        public static T GetEnumByValue<T>(this Enum value) where T : struct
        {
            return Enum.GetValues(typeof(T)).Cast<T>().SingleOrDefault(x => x.ToString() == value.ToString());
        }
        public static string ToShortString(this string value, int numOfCharacters)
        {
            if (!String.IsNullOrWhiteSpace(value))
            {
                if (value.Length > numOfCharacters)
                    return value.Substring(0, numOfCharacters) + "...";
                return value;
            }
            return String.Empty;
        }

        #endregion

        #region Feeds and Files

        public static string GetValidImageUrl(string imageUrl)
        {
            if (imageUrl.Contains("http://") || imageUrl.Contains("https://"))
                return imageUrl.Replace("https", "http");

            var baseUrl = "http://" + CurrentRequestData.CurrentSite.BaseUrl;
            if (CurrentRequestData.CurrentSite.BaseUrl.Contains("http://") || CurrentRequestData.CurrentSite.BaseUrl.Contains("https://"))
                baseUrl = CurrentRequestData.CurrentSite.BaseUrl.Replace("https", "http") + imageUrl;
            return baseUrl + imageUrl;
        }

        public static FileStream GetStream(AmazonEnvelope amazonEnvelope, AmazonEnvelopeMessageType amazonEnvelopeMessageType)
        {
            var xml = Serialize(amazonEnvelope);
            var fileLocation = GetFeedPath(amazonEnvelopeMessageType);
            File.WriteAllText(fileLocation, xml);
            return File.Open(fileLocation, FileMode.Open, FileAccess.Read);
        }

        private static string Serialize<T>(T item)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream))
                {
                    new XmlSerializer(typeof(T)).Serialize(writer, item);
                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }
        }
        private static string GetFeedPath(AmazonEnvelopeMessageType amazonEnvelopeMessageType)
        {
            return string.Format("{0}/Amazon{1}Feed-{2}.xml", GetDirPath(), amazonEnvelopeMessageType,
                                 CurrentRequestData.Now.ToString("yyyy-MM-dd hh-mm-ss"));
        }
        private static string GetDirPath()
        {
            var relativeFilePath = Path.Combine("/content/upload/", string.Format("{0}/{1}", CurrentRequestData.CurrentSite.Id, "amazon"));
            var baseDirectory = HostingEnvironment.ApplicationPhysicalPath.Substring(0, HostingEnvironment.ApplicationPhysicalPath.Length - 1);
            var path = Path.Combine(baseDirectory, relativeFilePath.Substring(1));

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public static void DeleteAmazonFilesOlderThan(TimeSpan timespan)
        {
            var amazonApiDir = GetDirPath();

            DeleteFilesOlderThan(timespan, amazonApiDir, "*.xml");
        }
        private static bool DeleteFilesOlderThan(TimeSpan timespan, string dir, string extension)
        {
            var files = Directory.GetFiles(dir, extension);

            foreach (var file in files)
            {
                try
                {
                    var fileInfo = new FileInfo(file);

                    if (fileInfo.CreationTimeUtc > DateTime.UtcNow.Add(timespan)) continue;

                    fileInfo.Delete();
                }
                catch (Exception ex)
                {
                    CurrentRequestData.ErrorSignal.Raise(ex);
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Settings

        public static bool CheckAppSettingsStatus(AmazonAppSettings settings)
        {
            return settings.GetType().GetProperties().Where(info => info.CanWrite && info.Name != "Site").All(property => !String.IsNullOrWhiteSpace(Convert.ToString(property.GetValue(settings, null))));
        }
        public static bool CheckSellerSettingsStatus(AmazonSellerSettings settings)
        {
            return settings.GetType().GetProperties().Where(info => info.CanWrite && info.Name != "Site").All(property => !String.IsNullOrWhiteSpace(Convert.ToString(property.GetValue(settings, null))));
        }

        #endregion
    }
}