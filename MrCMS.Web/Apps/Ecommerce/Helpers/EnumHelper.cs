using System;
using System.ComponentModel;
using System.Reflection;

namespace MrCMS.Web.Apps.Ecommerce.Helpers
{
    public static class EnumHelper
    {
        public static string GetDescription(this Enum enumVal)
        {
            var fi = enumVal.GetType().GetField(enumVal.ToString());

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0
                       ? attributes[0].Description
                       : enumVal.ToString();
        }
    }
}