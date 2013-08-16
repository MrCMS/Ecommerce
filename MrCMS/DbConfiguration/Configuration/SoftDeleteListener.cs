using MrCMS.Entities;
using MrCMS.Entities.Documents;
using MrCMS.Tasks;
using NHibernate.Event.Default;

namespace MrCMS.DbConfiguration.Configuration
{
    public class SoftDeleteListener : DefaultDeleteEventListener
    {
        protected override void DeleteEntity(NHibernate.Event.IEventSource session, object entity, NHibernate.Engine.EntityEntry entityEntry, bool isCascadeDeleteEnabled, NHibernate.Persister.Entity.IEntityPersister persister, Iesi.Collections.ISet transientEntities)
        {
            var systemEntity = entity as SystemEntity;
            if (systemEntity != null)
            {
                systemEntity.IsDeleted = true;

                CascadeBeforeDelete(session, persister, systemEntity, entityEntry, transientEntities);
                CascadeAfterDelete(session, persister, systemEntity, transientEntities);

                var siteEntity = systemEntity as SiteEntity;
                if (siteEntity != null)
                    TaskExecutor.ExecuteLater(UpdateIndicesListener.Create(typeof(DeleteIndicesTask<>), siteEntity));
            }
            else
            {
                base.DeleteEntity(session, entity, entityEntry, isCascadeDeleteEnabled,
                                  persister, transientEntities);
            }
        }
    }
}