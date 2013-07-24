using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class ProductAttributeValue : SiteEntity
    {
        public virtual ProductAttributeOption ProductAttributeOption { get; set; }
        public virtual string Value { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }

        public virtual int DisplayOrder { get { return ProductAttributeOption != null ? ProductAttributeOption.DisplayOrder : int.MaxValue; } }
        public virtual string FormattedValue
        {
            get { return ProductAttributeOption != null ? string.Format("{0} - {1}", ProductAttributeOption.Name, Value) : Value; }
        }
    }
}