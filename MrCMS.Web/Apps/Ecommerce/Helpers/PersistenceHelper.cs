using MrCMS.Entities;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Helpers
{
    public static class PersistenceHelper
    {
        public static T PersistTo<T>(this T entity, ISession session) where T : SystemEntity
        {
            if (entity == null || session == null)
                return entity;

            session.Transact(s => s.SaveOrUpdate(entity));
            return entity;
        }
    }
}