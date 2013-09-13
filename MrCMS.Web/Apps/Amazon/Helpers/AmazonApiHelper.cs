using System;
using System.Collections.Specialized;
using System.Linq;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Helpers
{
    public static class AmazonApiHelper
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
        #endregion
    }
}