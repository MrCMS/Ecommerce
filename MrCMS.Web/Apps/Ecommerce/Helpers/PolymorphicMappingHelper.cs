using FluentNHibernate.Mapping;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Helpers
{
    public static class PolymorphicMappingHelper
    {
        public static AnyPart<T> AutoMap<T>(this AnyPart<T> part, string discriminatorColumnName, string identifierColumnName)
        {
            var anyPart = part.EntityTypeColumn(discriminatorColumnName).EntityIdentifierColumn(identifierColumnName).IdentityType<int>();

            foreach (var type in TypeHelper.GetMappedClassesAssignableFrom<T>())
            {
                anyPart.AddMetaValue(type, type.FullName);
            }
            return anyPart;
        }
    }
}