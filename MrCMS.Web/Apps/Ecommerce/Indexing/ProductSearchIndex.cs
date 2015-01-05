using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Indexing
{
    public class ProductSearchIndex : IndexDefinition<Product>
    {
        private readonly HashSet<IFieldDefinition<ProductSearchIndex, Product>> _definitions;

        public ProductSearchIndex(ISession session,
            IEnumerable<IFieldDefinition<ProductSearchIndex, Product>> definitions)
            : base(session)
        {
            _definitions = new HashSet<IFieldDefinition<ProductSearchIndex, Product>>(definitions);
        }

        public override IEnumerable<FieldDefinition<Product>> Definitions
        {
            get { return _definitions.Select(definition => definition.GetDefinition); }
        }

        public override IEnumerable<string> FieldNames
        {
            get { return _definitions.Select(definition => definition.Name); }
        }

        public override string IndexFolderName
        {
            get { return "ProductIndex"; }
        }

        public override string IndexName
        {
            get { return "Product Search Index"; }
        }

        public override IEnumerable<IFieldDefinitionInfo> DefinitionInfos
        {
            get { return _definitions; }
        }

        protected override IEnumerable<IFieldable> GetAdditionalFields(Product entity)
        {
            IList<CategoryProductDisplayOrder> orders =
                _session.QueryOver<CategoryProductDisplayOrder>()
                    .Where(order => order.Product.Id == entity.Id)
                    .Cacheable()
                    .List();

            return orders.Select(GetCategoryField);
        }

        private static IFieldable GetCategoryField(CategoryProductDisplayOrder order)
        {
            var numericField = new NumericField(GetCategoryFieldName(order.Category.Id), Field.Store.YES, true);
            numericField.SetIntValue(order.DisplayOrder);
            return numericField;
        }

        public static string GetCategoryFieldName(int categoryId)
        {
            return "category-" + categoryId;
        }

        protected override Dictionary<Product, IEnumerable<IFieldable>> GetAdditionalFields(List<Product> entities)
        {
            HashSet<CategoryProductDisplayOrder> orders = _session.QueryOver<CategoryProductDisplayOrder>()
                .Cacheable()
                .List().ToHashSet();

            return entities.ToDictionary(product => product,
                product =>
                    orders.FindAll(order => order.Product.Id == product.Id).Select(GetCategoryField));
        }
    }
}