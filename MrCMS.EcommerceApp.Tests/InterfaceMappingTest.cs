using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Entities;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.EcommerceApp.Tests
{
    public class InterfaceMappingTest : InMemoryDatabaseTest
    {
        [Fact]
        public void CanMapToAnInterfaceWithValidOverrideTest()
        {
            var interfaceTest1 = new InterfaceTest1 {Value = 1};
            var interfaceTest2 = new InterfaceTest2 {Value = 1, OtherValue = 2};
            Session.Transact(session =>
                                 {
                                     session.Save(interfaceTest1);
                                     session.Save(interfaceTest2);
                                 });
            var class1 = new ClassWithInterface {Test = interfaceTest1};
            var class2 = new ClassWithInterface {Test = interfaceTest2};

            Session.Transact(session =>
                                 {
                                     Session.Save(class1);
                                     Session.Save(class2);
                                 });
        }
    }

    public class ClassWithInterfaceOverride : IAutoMappingOverride<ClassWithInterface>
    {
        public void Override(AutoMapping<ClassWithInterface> mapping)
        {
            mapping.ReferencesAny(withInterface => withInterface.Test)
                   .EntityIdentifierColumn("InterfaceId")
                   .EntityTypeColumn("InterfaceType")
                   .IdentityType<int>()
                   .AddMetaValue<InterfaceTest1>("it1")
                   .AddMetaValue<InterfaceTest2>("it2").Cascade.All();
        }
    }

    public class ClassWithInterface : SystemEntity
    {
        public virtual IInterfaceTest Test { get; set; }
    }

    public interface IInterfaceTest
    {
        int Id { get; set; }
        int Value { get; set; }
    }

    public class InterfaceTest2 : SystemEntity, IInterfaceTest
    {
        public virtual int Value { get; set; }
        public virtual int OtherValue { get; set; }
    }
    public class InterfaceTest1 : SystemEntity, IInterfaceTest
    {
        public virtual int Value { get; set; }
    }
}