using MrCMS.Entities;

namespace MrCMS.EcommerceApp.Tests.Builders
{
    public interface IEntityBuilder<T> where T : SystemEntity
    {
        T Build();
    }
}