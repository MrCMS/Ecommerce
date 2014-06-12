namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class ProductValueModel<T>
    {
        public string Name { get; set; }
        public T Id { get; set; }
    }
}