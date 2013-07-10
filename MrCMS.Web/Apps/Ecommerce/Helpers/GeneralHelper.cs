using System;
using System.ComponentModel;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Helpers
{
    public static class GeneralHelper
    {
        public static string GetDescriptionFromEnum(Enum value)
        {
            var attribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .SingleOrDefault() as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static bool IsValidLength(this string value, int maxLength, int minLength = 0)
        {
            if (!String.IsNullOrWhiteSpace(value))
            {
                if (minLength == 0)
                    return value.Length <= maxLength;
                else
                    return value.Length <= maxLength && value.Length >= minLength;
            }
            else
                return true;
        }
        public static bool HasValue(this string value)
        {
            return !String.IsNullOrWhiteSpace(value);
        }
        public static bool IsValidUrl(this string value)
        {
            return true;
            //var uri=new Uri(String.Empty);
            //return Uri.TryCreate(value,UriKind.Absolute,out uri);
        }
    }
}