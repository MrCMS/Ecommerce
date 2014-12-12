using System;
using System.Collections.Generic;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Events.Documents;
using MrCMS.Search;

namespace MrCMS.Services.Notifications
{
    public class NotificationDisabler : IDisposable
    {
        private List<IDisposable> _disposables;

        public NotificationDisabler()
        {
            _disposables = new List<IDisposable>
            {
                EventContext.Instance.Disable<IOnTransientNotificationPublished>(),
                EventContext.Instance.Disable<IOnPersistentNotificationPublished>(),
                EventContext.Instance.Disable<UpdateIndicesListener>(),
                EventContext.Instance.Disable<UpdateUniversalSearch>(),
                EventContext.Instance.Disable<WebpageUpdatedNotification>(),
                EventContext.Instance.Disable<DocumentAddedNotification>(),
                EventContext.Instance.Disable<MediaCategoryUpdatedNotification>()
            };

        }
        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}