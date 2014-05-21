using System.Collections.Generic;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using System.Linq;

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
    }
}