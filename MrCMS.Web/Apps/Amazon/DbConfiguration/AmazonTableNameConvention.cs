using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using Lucene.Net.Documents;
using MrCMS.Apps;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.People;
using MrCMS.Entities.Widget;

namespace MrCMS.Web.Apps.Amazon.DbConfiguration
{
    public class AmazonTableNameConvention : IClassConvention
    {
        public void Apply(IClassInstance instance)
        {
            var entityType = instance.EntityType;
            if (MrCMSApp.AllAppTypes.ContainsKey(entityType) && MrCMSApp.AllAppTypes[entityType] == AmazonApp.AmazonAppName)
            {
                if (!entityType.IsSubclassOf(typeof(Document)) &&
                    !entityType.IsSubclassOf(typeof(UserProfileData)) && !entityType.IsSubclassOf(typeof(Widget)))
                {
                    instance.Table(string.Format(AmazonApp.AmazonAppName+"_{0}", instance.TableName.Replace("`", "")));
                }
            }
        }

    }
}
