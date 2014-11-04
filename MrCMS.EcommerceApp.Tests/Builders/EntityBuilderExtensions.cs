using MrCMS.Entities;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.EcommerceApp.Tests.Builders
{
    public static class EntityBuilderExtensions
    {
        public static T BuildAndPersist<T>(this IEntityBuilder<T> builder, ISession session) where T : SystemEntity
        {
            var entity = builder.Build();
            session.Transact(s => s.Save(entity));
            return entity;
        }
    }
}