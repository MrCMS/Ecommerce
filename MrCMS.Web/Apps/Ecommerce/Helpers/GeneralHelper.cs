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
        public static bool HasValue(this string value)
        {
            return !String.IsNullOrWhiteSpace(value);
        }
        public static bool IsConvertable<T>(this string value) where T : struct
         {
             try
             {
                 if (value.HasValue())
                 {
                     var convertedValue=Convert.ChangeType(value, typeof(T));
                 }
                 return true;
             }
             catch (Exception)
             {
                 return false;
             }
         }
    }
}