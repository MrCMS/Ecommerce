using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using Lucene.Net.Documents;
using MrCMS.Apps;
using MrCMS.Entities.People;
using MrCMS.Entities.Widget;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class TableNameConvention : IClassConvention, IHasManyToManyConvention
    {
        public void Apply(IClassInstance instance)
        {
            Type entityType = instance.EntityType;
            if (IsEcommerceType(entityType))
            {
                instance.Table(GetTableName(instance.TableName));
            }
        }

        public void Apply(IManyToManyCollectionInstance instance)
        {
            Type entityType = instance.EntityType;
            if (IsEcommerceType(entityType))
            {
                instance.Table(GetTableName(instance.TableName));
            }
        }

        private static bool IsEcommerceType(Type entityType)
        {
            return MrCMSApp.AllAppTypes.ContainsKey(entityType) &&
                   MrCMSApp.AllAppTypes[entityType] == EcommerceApp.EcommerceAppName &&
                   (!entityType.IsSubclassOf(typeof (Document)) &&
                    !entityType.IsSubclassOf(typeof (UserProfileData)) && !entityType.IsSubclassOf(typeof (Widget)));
        }

        private static string GetTableName(string tableName)
        {
            return string.Format("Ecommerce_{0}", tableName.Replace("`", ""));
        }
    }
}