using System;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using MrCMS.Entities.Multisite;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website;
using NHibernate;
using System.Linq;
using MrCMS.Indexing.Utils;
using MrCMS.Helpers;
using NHibernate.Criterion;
using Version = Lucene.Net.Util.Version;

namespace MrCMS.Web.Apps.Ecommerce.Indexing
{
    public class ProductSearchIndex : IIndexDefinition<Product>,
                                      IRelatedItemIndexDefinition<ProductVariant, Product>,
                                      IRelatedItemIndexDefinition<ProductSpecificationAttribute, Product>,
                                      IRelatedItemIndexDefinition<ProductSpecificationAttributeOption, Product>,
                                      IRelatedItemIndexDefinition<ProductSpecificationValue, Product>,
                                      IRelatedItemIndexDefinition<ProductOption, Product>,
                                      IRelatedItemIndexDefinition<ProductOptionValue, Product>,
                                      IRelatedItemIndexDefinition<Category, Product>
    {
        public Document Convert(Product entity)
        {
            return new Document().SetFields(Definitions, entity);
        }

        public Term GetIndex(Product entity)
        {
            return new Term("id", entity.Id.ToString());
        }

        public Product Convert(ISession session, Document document)
        {
            var id = document.GetValue<int>("id");
            return session.Get<Product>(id);
        }

        public IEnumerable<Product> Convert(ISession session, IEnumerable<Document> documents)
        {
            var ids = documents.Select(document => document.GetValue<int>("id")).ToList();
            var products =
                ids.Chunk(100)
                   .SelectMany(
                       ints => session.QueryOver<Product>().Where(product => product.Id.IsIn(ints.ToList())).List());

            return products.OrderBy(product => ids.IndexOf(product.Id));
        }

        public string GetLocation(Site currentSite)
        {
            var location = string.Format("~/App_Data/Indexes/{0}/Products/", currentSite.Id);
            var mapPath = CurrentRequestData.CurrentContext.Server.MapPath(location);
            return mapPath;
        }

        public Analyzer GetAnalyser()
        {
            return new StandardAnalyzer(Version.LUCENE_30);
        }

        public string IndexName
        {
            get { return "Product Search Index"; }
        }

        public IEnumerable<FieldDefinition<Product>> Definitions
        {
            get
            {
                yield return Id;
                yield return Name;
                yield return NameSort;
                yield return BodyContent;
                yield return MetaTitle;
                yield return MetaKeywords;
                yield return MetaDescription;
                yield return UrlSegment;
                yield return PublishOn;
                yield return CreatedOn;
                yield return Price;
                yield return Specifications;
                yield return Options;
                yield return Categories;
                yield return Brand;
                yield return SKUs;
                yield return NumberBought;
            }
        }

        public static FieldDefinition<Product> Id
        {
            get { return _id; }
        }

        public static FieldDefinition<Product> Name
        {
            get { return _name; }
        }

        public static FieldDefinition<Product> NameSort
        {
            get { return _nameSort; }
        }

        public static FieldDefinition<Product> BodyContent
        {
            get { return _bodyContent; }
        }

        public static FieldDefinition<Product> MetaTitle
        {
            get { return _metaTitle; }
        }

        public static FieldDefinition<Product> MetaKeywords
        {
            get { return _metaKeywords; }
        }

        public static FieldDefinition<Product> MetaDescription
        {
            get { return _metaDescription; }
        }

        public static FieldDefinition<Product> UrlSegment
        {
            get { return _urlSegment; }
        }

        public static FieldDefinition<Product> PublishOn
        {
            get { return _publishOn; }
        }

        public static FieldDefinition<Product> CreatedOn
        {
            get { return _createdOn; }
        }

        public static DecimalFieldDefinition<Product> Price
        {
            get { return _price; }
        }

        public static FieldDefinition<Product> Specifications
        {
            get { return _specifications; }
        }

        public static FieldDefinition<Product> Options
        {
            get { return _options; }
        }

        public static FieldDefinition<Product> Categories
        {
            get { return _categories; }
        }

        public static FieldDefinition<Product> Brand
        {
            get { return _brand; }
        }

        public static FieldDefinition<Product> SKUs
        {
            get { return _skus; }
        }

        public static FieldDefinition<Product> NumberBought
        {
            get { return _numberBought; }
        }

        private static readonly FieldDefinition<Product> _id =
            new StringFieldDefinition<Product>("id", webpage => webpage.Id.ToString(), Field.Store.YES,
                                               Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Product> _name =
            new StringFieldDefinition<Product>("name", webpage => webpage.Name, Field.Store.YES,
                                               Field.Index.ANALYZED);

        private static readonly FieldDefinition<Product> _nameSort =
            new StringFieldDefinition<Product>("nameSort", webpage => webpage.Name.Trim().ToLower(), Field.Store.NO,
                                               Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Product> _bodyContent =
            new StringFieldDefinition<Product>("bodycontent", webpage => webpage.BodyContent, Field.Store.NO,
                                               Field.Index.ANALYZED);

        private static readonly FieldDefinition<Product> _metaTitle =
            new StringFieldDefinition<Product>("metatitle", webpage => webpage.MetaTitle, Field.Store.NO,
                                               Field.Index.ANALYZED);

        private static readonly FieldDefinition<Product> _metaKeywords =
            new StringFieldDefinition<Product>("metakeywords", webpage => webpage.MetaKeywords, Field.Store.NO,
                                               Field.Index.ANALYZED);

        private static readonly FieldDefinition<Product> _metaDescription =
            new StringFieldDefinition<Product>("metadescription",
                                               webpage => webpage.MetaDescription, Field.Store.NO, Field.Index.ANALYZED);

        private static readonly FieldDefinition<Product> _urlSegment =
            new StringFieldDefinition<Product>("urlsegment", webpage => webpage.UrlSegment, Field.Store.NO,
                                               Field.Index.ANALYZED);

        private static readonly FieldDefinition<Product> _publishOn =
            new StringFieldDefinition<Product>("publishOn",
                                               webpage =>
                                               DateTools.DateToString(
                                                   webpage.PublishOn.GetValueOrDefault(DateTime.MaxValue),
                                                   DateTools.Resolution.SECOND), Field.Store.YES,
                                               Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Product> _createdOn =
            new StringFieldDefinition<Product>("createdOn",
                                               webpage =>
                                               DateTools.DateToString(webpage.CreatedOn, DateTools.Resolution.SECOND), Field.Store.YES,
                                               Field.Index.NOT_ANALYZED);

        private static readonly DecimalFieldDefinition<Product> _price =
            new DecimalFieldDefinition<Product>("price", product => GetPrices(product), Field.Store.YES,
                                                Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Product> _specifications =
            new StringFieldDefinition<Product>("specifications",
                                               product =>
                                               product.SpecificationValues.Select(value => value.ProductSpecificationAttributeOption.Id.ToString())
                                                      .Distinct(),
                                               Field.Store.YES, Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Product> _options =
            new StringFieldDefinition<Product>("options",
                                               product =>
                                               product.Variants.SelectMany(
                                                   variant => variant.OptionValues.Select(value => value.Id))
                                                      .Select(i => i.ToString()),
                                               Field.Store.YES, Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Product> _categories =
            new StringFieldDefinition<Product>("categories",
                                               product =>
                                               GetCategories(product.Categories),
                                               Field.Store.YES, Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Product> _brand =
            new StringFieldDefinition<Product>("brand",
                                               product => GetBrand(product),
                                               Field.Store.YES, Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Product> _skus =
            new StringFieldDefinition<Product>("skus",
                                               product =>
                                               GetSKUs(product.Variants),
                                               Field.Store.YES, Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Product> _numberBought =
            new IntegerFieldDefinition<Product>("numberBought",
                                               product =>
                                               GetNumberBought(product.Variants),
                                               Field.Store.YES, Field.Index.NOT_ANALYZED);

        private static int GetNumberBought(IList<ProductVariant> variants)
        {
            var orderLines = MrCMSApplication.Get<ISession>().QueryOver<OrderLine>().Where(line => line.ProductVariant.IsIn(variants.ToList())).List();
            return orderLines.Sum(line => line.Quantity);
        }

        private static IEnumerable<string> GetBrand(Product product)
        {
            if (product.Brand != null)
                yield return product.Brand.Id.ToString();
        }

        private static IEnumerable<string> GetCategories(IEnumerable<Category> categories)
        {
            var list = new List<Category>();
            list.AddRange(categories.SelectMany(GetCategoryHierarchy));
            return list.Distinct().Select(category => category.Id.ToString());
        }

        private static IEnumerable<Category> GetCategoryHierarchy(Category category)
        {
            yield return category;
            var parent = category.Parent.Unproxy() as Category;
            while (parent != null)
            {
                yield return parent;
                parent = parent.Parent.Unproxy() as Category;
            }
        }

        private static IEnumerable<string> GetSKUs(IEnumerable<ProductVariant> productVariants)
        {
            return productVariants.Select(x => x.SKU);
        }

        public static IEnumerable<decimal> GetPrices(Product entity)
        {
            return entity.Variants.Select(pv => pv.Price);
        }

        public IEnumerable<Product> GetEntitiesToUpdate(ProductVariant obj)
        {
            yield return obj.Product;
        }

        public IEnumerable<Product> GetEntitiesToUpdate(ProductSpecificationAttribute obj)
        {
            return from productSpecificationAttributeOption in obj.Options
                   from productSpecificationValue in productSpecificationAttributeOption.Values
                   select productSpecificationValue.Product;
        }

        public IEnumerable<Product> GetEntitiesToUpdate(ProductSpecificationAttributeOption obj)
        {
            return obj.Values.Select(productSpecificationValue => productSpecificationValue.Product);
        }

        public IEnumerable<Product> GetEntitiesToUpdate(ProductOption obj)
        {
            return obj.Products;
        }

        public IEnumerable<Product> GetEntitiesToUpdate(ProductOptionValue obj)
        {
            if (obj.ProductVariant != null)
                yield return obj.ProductVariant.Product;
        }

        public IEnumerable<Product> GetEntitiesToUpdate(Category obj)
        {
            return obj.Products;
        }

        public IEnumerable<Product> GetEntitiesToUpdate(ProductSpecificationValue obj)
        {
            yield return obj.Product;
        }
    }
}