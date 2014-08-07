using System;
using System.Collections;
using Elmah;
using MrCMS.DbConfiguration.Types;
using MrCMS.Entities.Multisite;
using MrCMS.Website;
using NHibernate;
using MrCMS.Helpers;

namespace MrCMS.Logging
{
    public class MrCMSErrorLog : ErrorLog
    {

        public override string Name
        {
            get
            {
                return "MrCMS Database Error Log";
            }
        }

        public MrCMSErrorLog(IDictionary config)
        {
        }

        private ISession GetSession()
        {
            return CurrentRequestData.DatabaseIsInstalled ? MrCMSApplication.Get<ISessionFactory>().OpenFilteredSession() : null;
        }

        public override string Log(Error error)
        {
            var newGuid = Guid.NewGuid();

            var session = GetSession();
            if (session != null)
            {
                var log = new Log
                              {
                                  Error = BinaryData.CanSerialize(error) ? error : new Error(),
                                  Guid = newGuid,
                                  Message = error.Message,
                                  Detail = error.Detail,
                                  Site = session.Get<Site>(CurrentRequestData.CurrentSite.Id)
                              };
                session.Transact(ses => ses.Save(log));
            }

            return newGuid.ToString();
        }

        public override int GetErrors(int pageIndex, int pageSize, IList errorEntryList)
        {
            if (pageIndex < 0)
                throw new ArgumentOutOfRangeException("pageIndex", pageIndex, null);
            if (pageSize < 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, null);

            var session = GetSession();
            var errorLogEntries =
                session.QueryOver<Log>()
                        .Where(entry => entry.Type == LogEntryType.Error)
                        .OrderBy(entry => entry.CreatedOn).Desc
                        .Paged(pageIndex + 1, pageSize);
            errorLogEntries.ForEach(entry =>
                                    errorEntryList.Add(new ErrorLogEntry(this, entry.Guid.ToString(), entry.Error)));
            return errorLogEntries.TotalItemCount;
        }

        public override ErrorLogEntry GetError(string id)
        {
            Guid guid;
            try
            {
                guid = new Guid(id);
                id = guid.ToString();
            }
            catch (FormatException ex)
            {
                throw new ArgumentException(ex.Message, id, ex);
            }

            try
            {
                var session = GetSession();
                var logEntry = session.QueryOver<Log>().Where(entry => entry.Guid == guid).Cacheable().SingleOrDefault();
                return new ErrorLogEntry(this, id, logEntry.Error);
            }
            finally
            {
            }
        }
    }
}