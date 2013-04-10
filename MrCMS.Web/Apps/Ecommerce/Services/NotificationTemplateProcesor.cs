using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class NotificationTemplateProcesor : INotificationTemplateProcessor
    {
        private string ProcessedTemplate { get; set; }

        public NotificationTemplateProcesor()
        {

        }

        public string ReplaceTokensAndMethods<T>(T tokenProvider, string Template)
        {
            ProcessedTemplate = ReplaceTokens<T>(tokenProvider, Template);
            ProcessedTemplate = ReplaceMethods<T>(tokenProvider, ProcessedTemplate);
            ProcessedTemplate = ReplaceExtensionMethods<T>(tokenProvider, ProcessedTemplate);
            return ProcessedTemplate;
        }

        public string ReplaceExtensionMethods<T>(T tokenProvider, string Template)
        {
            var query = from type in tokenProvider.GetType().Assembly.GetTypes()
                        where type.IsSealed && !type.IsGenericType && !type.IsNested
                        from method in type.GetMethods(BindingFlags.Static
                            | BindingFlags.Public | BindingFlags.NonPublic)
                        where method.IsDefined(typeof(ExtensionAttribute), false)
                        where method.GetParameters()[0].ParameterType == tokenProvider.GetType()
                        select method;

            Dictionary<string, string> replacements = new Dictionary<string, string>();

            foreach (Match item in GetRegexMatches(Template))
            {
                if (item.Value.Contains("()"))
                {
                    string cleanMethodName = item.Value.Replace("{", "").Replace("}", "").Replace("(", "").Replace(")", ""); ;
                    MethodInfo method = query.Where(x => x.Name.Contains(cleanMethodName)).SingleOrDefault();
                    if (method != null)
                        replacements.Add(method.Name + "()", method.Invoke(tokenProvider,new object[]{tokenProvider}).ToString());
                }
            }

            return ReplaceTokensForString(Template, replacements);
        }

        public string ReplaceMethods<T>(T tokenProvider,string Template)
        {
            MethodInfo[] methods = tokenProvider.GetType().GetMethods();
            Dictionary<string, string> replacements = new Dictionary<string, string>();

            foreach (Match item in GetRegexMatches(Template))
            {
                if (item.Value.Contains("()"))
                {
                    string cleanMethodName = item.Value.Replace("{", "").Replace("}", "").Replace("(", "").Replace(")", ""); ;
                    MethodInfo method = methods.Where(x => x.Name.Contains(cleanMethodName)).SingleOrDefault();
                    if (method != null)
                        replacements.Add(method.Name + "()", method.Invoke(tokenProvider, null).ToString());
                }
            }

            return ReplaceTokensForString(Template, replacements);
        }

        public string ReplaceTokens<T>(T tokenProvider, string Template)
        {
            Type[] acceptedTypes = { typeof(String), typeof(Int32), typeof(Decimal), typeof(DateTime), typeof(Boolean), typeof(bool), typeof(float) };
            Dictionary<string, string> replacements = new Dictionary<string, string>();
            foreach (PropertyInfo item in tokenProvider.GetType().GetProperties())
            {
                if (acceptedTypes.Where(x => x == item.PropertyType).Count() > 0)
                {
                    object value = item.GetValue(tokenProvider, null);
                    if (value != null)
                    {
                        replacements.Add(item.Name, value.ToString());
                    }
                }
            }

            return ReplaceTokensForString(Template, replacements);
        }

        public string ReplaceTokensForString(string template, Dictionary<string, string> replacements)
        {
            var regex = new Regex(@"\{([^}]+)}");
            return (regex.Replace(template, delegate(Match match)
            {
                string key = match.Groups[1].Value;
                string replacement = replacements.ContainsKey(key) ? replacements[key] : match.Value;
                return (replacement);
            }));
        }

        public MatchCollection GetRegexMatches(string template)
        {
            var regex = new Regex(@"\{([^}]+)}");
            return regex.Matches(template);
        }
    }
}