using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace MrCMS.Web.Apps.Ecommerce.Services.SagePay
{
    /// <summary>
    /// Used for serializing an object for use with an HTTP POST.
    /// </summary>
    public class HttpPostSerializer
    {
        /// <summary>
        /// Serializes an object to a format usable for an HTTP POST. 
        /// All public instance properties are serialized. 
        /// </summary>
        public string Serialize(object toSerialize)
        {
            var type = toSerialize.GetType();
            var pairs = new Dictionary<string, string>();

            foreach (var property in GetProperties(type))
            {
                if (!property.CanRead) continue;

                var rawValue = property.GetValue(toSerialize, null);
                if (rawValue == null && IsOptional(property)) continue;

                var format = GetFormat(property);
                // Always use EN-GB
                string convertedValue = string.Format(CultureInfo.InvariantCulture, format, rawValue);

                if (ShouldEncode(property))
                {
                    convertedValue = HttpUtility.UrlEncode(convertedValue, Encoding.GetEncoding("ISO-8859-15"));
                }

                pairs.Add(property.Name, convertedValue);
            }

            var result = from pair in pairs
                         select pair.Key + "=" + pair.Value;

            return string.Join("&", result.ToArray());
        }

        static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod);
        }

        static string GetFormat(PropertyInfo property)
        {
            var attribute = (FormatAttribute)Attribute.GetCustomAttribute(property, typeof(FormatAttribute));

            if (attribute != null)
            {
                return "{0:" + attribute.Format + "}";
            }

            return "{0}";
        }

        static bool ShouldEncode(PropertyInfo property)
        {
            return !Attribute.IsDefined(property, typeof(UnencodedAttribute));
        }

        static bool IsOptional(PropertyInfo property)
        {
            return Attribute.IsDefined(property, typeof(OptionalAttribute));
        }
    }
}