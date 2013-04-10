using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface INotificationTemplateProcessor
    {
        string ReplaceTokensAndMethods<T>(T tokenProvider,string Template);
        string ReplaceExtensionMethods<T>(T tokenProvider, string Template);
        string ReplaceMethods<T>(T tokenProvider, string Template);
        string ReplaceTokens<T>(T tokenProvider, string Template);
        string ReplaceTokensForString(string template, Dictionary<string, string> replacements);
        MatchCollection GetRegexMatches(string template);
    }
}
