using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class ProductOptionValue : SiteEntity
    {
        public virtual ProductOption ProductOption { get; set; }
        public virtual string Value { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }

        public virtual int DisplayOrder
        {
            get
            {
                return ProductOption != null && ProductVariant != null
                    ? ProductVariant.Product.Options.IndexOf(ProductOption)
                    : int.MaxValue;
            }
        }

        public virtual string FormattedValue
        {
            get { return ProductOption != null ? string.Format("{0} - {1}", ProductOption.Name, Value) : Value; }
        }
    }
}