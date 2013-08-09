using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MrCMS.Web.Apps.Ecommerce.Services.MessageTemplates
{
    public interface IMessageTemplateProcessor
    {
        string ReplaceTokensAndMethods<T>(T tokenProvider,string template);
        string ReplaceExtensionMethods<T>(T tokenProvider, string template);
        string ReplaceMethods<T>(T tokenProvider, string template);
        string ReplaceTokens<T>(T tokenProvider, string template);
        string ReplaceTokensForString(string template, Dictionary<string, string> replacements);
        MatchCollection GetRegexMatches(string template);
    }
}
