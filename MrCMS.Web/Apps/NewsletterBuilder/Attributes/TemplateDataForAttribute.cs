using System;

namespace MrCMS.Web.Apps.NewsletterBuilder.Attributes
{
    public class TemplateDataForAttribute : Attribute
    {
        public TemplateDataForAttribute(Type type)
        {
            Type = type;
        }

        public Type Type { get; set; }
    }
}