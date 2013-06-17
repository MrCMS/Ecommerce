using System;
using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Services;
using MrCMS.Tasks;
using MrCMS.Website;
using NHibernate.Event;
using System.Linq;

namespace MrCMS.DbConfiguration.Configuration
{
    public class UpdateIndexesListener : IPostUpdateEventListener, IPostInsertEventListener, IPostDeleteEventListener
    {
        public void OnPostUpdate(PostUpdateEvent @event)
        {
            TaskExecutor.ExecuteLater(new UpdateIndexesTask(@event.Entity as SiteEntity));
        }

        public void OnPostInsert(PostInsertEvent @event)
        {
            TaskExecutor.ExecuteLater(new InsertIndexesTask(@event.Entity as SiteEntity));
        }

        public void OnPostDelete(PostDeleteEvent @event)
        {
            TaskExecutor.ExecuteLater(new DeleteIndexesTask(@event.Entity as SiteEntity));
        }
    }

    public class DeleteIndexesTask : IndexManagementTask
    {
        public DeleteIndexesTask(SiteEntity entity) : base(entity)
        {
        }

        protected override void ExecuteLogic()
        {
            var definitionTypes = GetDefinitionTypes(Entity.GetType());
            foreach (var indexManagerBase in definitionTypes.Select(IndexService.GetIndexManagerBase))
                indexManagerBase.Delete(Entity);
        }
    }

    public abstract class IndexManagementTask : BackgroundTask
    {
        protected readonly SiteEntity Entity;

        protected IndexManagementTask(SiteEntity entity)
        {
            Entity = entity;
        }
        protected List<Type> GetDefinitionTypes(Type entityType)
        {
            var indexDefinitionTypes = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(IIndexDefinition<>));
            var definitionTypes = indexDefinitionTypes.Where(type =>
                                                                 {
                                                                     var indexDefinitionInterface =
                                                                         type.GetInterfaces()
                                                                             .FirstOrDefault(
                                                                                 interfaceType =>
                                                                                 interfaceType.IsGenericType &&
                                                                                 interfaceType.GetGenericTypeDefinition() ==
                                                                                 typeof(IIndexDefinition<>));
                                                                     var genericArgument =
                                                                         indexDefinitionInterface.GetGenericArguments()[
                                                                             0];

                                                                     return
                                                                         genericArgument.IsAssignableFrom(entityType);
                                                                 }).ToList();
            return definitionTypes;
        }

        public override void Execute()
        {
            if (Entity != null)
            {
                CurrentRequestData.SetTaskSite(Session.Get<Site>(Entity.Site.Id));
                ExecuteLogic();
                CurrentRequestData.SetTaskSite(null);
            }
        }

        protected abstract void ExecuteLogic();
    }

    public class UpdateIndexesTask : IndexManagementTask
    {
        public UpdateIndexesTask(SiteEntity entity)
            : base(entity)
        {
        }

        protected override void ExecuteLogic()
        {
            var definitionTypes = GetDefinitionTypes(Entity.GetType());
            foreach (var indexManagerBase in definitionTypes.Select(IndexService.GetIndexManagerBase))
                indexManagerBase.Update(Entity);
        }
    }

    public class InsertIndexesTask : IndexManagementTask
    {
        public InsertIndexesTask(SiteEntity entity)
            : base(entity)
        {
        }

        protected override void ExecuteLogic()
        {
            var definitionTypes = GetDefinitionTypes(Entity.GetType());
            foreach (var indexManagerBase in definitionTypes.Select(IndexService.GetIndexManagerBase))
                indexManagerBase.Insert(Session.Get(Entity.GetType(), Entity.Id));
        }
    }
}