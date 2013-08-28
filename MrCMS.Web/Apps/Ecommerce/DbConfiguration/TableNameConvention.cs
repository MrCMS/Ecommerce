using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using Lucene.Net.Documents;
using MrCMS.Apps;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.People;
using MrCMS.Entities.Widget;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class TableNameConvention : IClassConvention
    {
        public void Apply(IClassInstance instance)
        {
            var entityType = instance.EntityType;
            if (MrCMSApp.AllAppTypes.ContainsKey(entityType) && MrCMSApp.AllAppTypes[entityType] == EcommerceApp.EcommerceAppName)
            {
                if (!entityType.IsSubclassOf(typeof(Document)) && !entityType.IsSubclassOf(typeof(MessageTemplate)) &&
                    !entityType.IsSubclassOf(typeof(UserProfileData)) && !entityType.IsSubclassOf(typeof(Widget)))
                {
                    instance.Table(string.Format("Ecommerce_{0}", instance.TableName.Replace("`", "")));
                }
            }
        }

    }
}
