using System;
using System.Collections.Generic;
using System.Reflection;
using MrCMS.DbConfiguration;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Tasks;
using MrCMS.Web.Tests.Stubs;
using MrCMS.Website;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace MrCMS.Web.Tests
{
    public abstract class InMemoryDatabaseTest : MrCMSTest
    {
        private static Configuration Configuration;
        private static ISessionFactory SessionFactory;
        protected static ISession Session;
        private readonly object lockObject = new object();

        protected InMemoryDatabaseTest()
        {
            if (Configuration == null)
            {
                lock (lockObject)
                {
                    var assemblies = new List<Assembly> { typeof(StubWebpage).Assembly };
                    var nHibernateModule = new NHibernateConfigurator
                                               {
                                                   CacheEnabled = true,
                                                   DatabaseType = DatabaseType.Sqlite,
                                                   InDevelopment = true,
                                                   ManuallyAddedAssemblies = assemblies
                                               };
                    Configuration = nHibernateModule.GetConfiguration();

                    SessionFactory = Configuration.BuildSessionFactory();
                }
            }

            Session = SessionFactory.OpenFilteredSession();

            new SchemaExport(Configuration).Execute(false, true, false, Session.Connection, null);

            SetupUser();


            CurrentSite = Session.Transact(session =>
                {
                    var site = new Site { Name = "Current Site", BaseUrl = "www.currentsite.com" };
                    CurrentRequestData.CurrentSite = site;
                    session.SaveOrUpdate(site);
                    return site;
                });

            CurrentRequestData.SiteSettings = new SiteSettings { TimeZone = TimeZoneInfo.Local.Id };

            TaskExecutor.Discard();
        }

        protected Site CurrentSite { get; set; }



        private void SetupUser()
        {
            var user = new User
                           {
                               Email = "test@example.com",
                               IsActive = true,
                           };

            var adminUserRole = new UserRole
                                    {
                                        Name = UserRole.Administrator
                                    };

            user.Roles = new List<UserRole> { adminUserRole };
            adminUserRole.Users = new List<User> { user };

            CurrentRequestData.CurrentUser = user;
        }

        public override void Dispose()
        {
            Dispose(true);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (Session != null)
                {
                    Session.Dispose();
                    Session = null;
                }
                base.Dispose();
            }
        }
    }
}