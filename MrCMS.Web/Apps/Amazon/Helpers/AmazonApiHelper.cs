using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Serialization;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Helpers
{
    public static class AmazonApiHelper
    {
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
        public static string ToTokenizedString(this StringCollection collection, string token)
        {
            return collection.Count > 1 ?
                collection.Cast<string>().Aggregate(String.Empty, (current, item) => current + (item + token)) :
                collection.Cast<string>().Aggregate(String.Empty, (current, item) => current + item);
        }
        public static string GenerateImageUrl(string imageUrl)
        {
            var siteUrl = "http://" + CurrentRequestData.CurrentSite.BaseUrl;

            return (!imageUrl.Contains("http") && !imageUrl.Contains("https"))
                                   ? (siteUrl + imageUrl)
                                   : imageUrl;
        }
        public static string Serialize<T>(T item)
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
        public static string GetAmazonApiFolderPath(string relativeFilePath)
        {
            if (!relativeFilePath.StartsWith("/content/upload/") && !relativeFilePath.StartsWith("/" + "/content/upload/"))
                relativeFilePath = Path.Combine("/content/upload/", relativeFilePath);
            var baseDirectory = HostingEnvironment.ApplicationPhysicalPath.Substring(0, HostingEnvironment.ApplicationPhysicalPath.Length - 1);
            var path = Path.Combine(baseDirectory, relativeFilePath.Substring(1));
            return path;
        }
    }
}