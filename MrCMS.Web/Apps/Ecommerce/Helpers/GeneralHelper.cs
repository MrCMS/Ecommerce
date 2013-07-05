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

        public static T ChangeTypeFromString<T>(string value) where T : struct
        {
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception)
            {
                return default(T);
            }

        }
    }
}