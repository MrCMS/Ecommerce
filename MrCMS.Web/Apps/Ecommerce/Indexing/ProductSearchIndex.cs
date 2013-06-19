using System;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using MrCMS.Entities.Multisite;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website;
using NHibernate;
using System.Linq;
using MrCMS.Indexing.Utils;
using MrCMS.Helpers;
using NHibernate.Criterion;
using Version = Lucene.Net.Util.Version;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using System.Collections;

namespace MrCMS.Web.Apps.Ecommerce.Indexing
{
    public class ProductSearchIndex : IIndexDefinition<Product>
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

        public string GetLocation(CurrentSite currentSite)
        {
            var location = string.Format("~/App_Data/Indexes/{0}/Products/", currentSite.Id);
            var mapPath = CurrentRequestData.CurrentContext.Server.MapPath(location);
            return mapPath;
        }

        public Analyzer GetAnalyser()
        {
            return new StandardAnalyzer(Version.LUCENE_30);
        }

        public string IndexName { get { return "Product Search Index"; } }

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
                yield return Price;
                yield return Specifications;
                yield return Options;
                yield return Categories;
            }
        }
        public static FieldDefinition<Product> Id { get { return _id; } }
        public static FieldDefinition<Product> Name { get { return _name; } }
        public static FieldDefinition<Product> NameSort { get { return _nameSort; } }
        public static FieldDefinition<Product> BodyContent { get { return _bodyContent; } }
        public static FieldDefinition<Product> MetaTitle { get { return _metaTitle; } }
        public static FieldDefinition<Product> MetaKeywords { get { return _metaKeywords; } }
        public static FieldDefinition<Product> MetaDescription { get { return _metaDescription; } }
        public static FieldDefinition<Product> UrlSegment { get { return _urlSegment; } }
        public static FieldDefinition<Product> PublishOn { get { return _publishOn; } }
        public static DecimalFieldDefinition<Product> Price { get { return _price; } }
        public static FieldDefinition<Product> Specifications { get { return _specifications; } }
        public static FieldDefinition<Product> Options { get { return _options; } }
        public static FieldDefinition<Product> Categories { get { return _categories; } }

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
                                         DateTools.DateToString(webpage.PublishOn.GetValueOrDefault(DateTime.MaxValue),
                                                                DateTools.Resolution.SECOND), Field.Store.NO,
                                         Field.Index.NOT_ANALYZED);

        private static readonly DecimalFieldDefinition<Product> _price =
            new DecimalFieldDefinition<Product>("price", webpage => GetPrice(webpage), Field.Store.NO,
                                         Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Product> _specifications =
          new StringFieldDefinition<Product>("specifications",
                                         product =>
                                         product.SpecificationValues.Distinct().Select(specification => GetSpecificationValue(specification)),
                                       Field.Store.NO, Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Product> _options =
            new StringFieldDefinition<Product>("options",
                                               product =>
                                               product.Variants.Distinct().Select(option => GetOptionValues(option.AttributeValues)),
                                               Field.Store.NO, Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Product> _categories =
            new StringFieldDefinition<Product>("categories",
                                               product =>
                                               product.Categories.Distinct().Select(category => category.Id.ToString()),
                                               Field.Store.NO, Field.Index.NOT_ANALYZED);

        public static decimal GetPrice(Product entity)
        {
            return !entity.Variants.Any() ? entity.Price : entity.Variants.Select(pv => pv.Price).Min();
        }

        public static string GetOptionValues(IList<ProductAttributeValue> values)
        {
            string optionValues = String.Empty;
            if (values.Count() > 0)
            {
                foreach (var item in values)
                {
                    optionValues += item.ProductAttributeOption.Name.ToLower() + item.Value.ToLower() + ",";
                }
            }
            return optionValues;
        }

        public static string GetSpecificationValue(ProductSpecificationValue entity)
        {
            if (entity != null)
                return entity.ProductSpecificationAttribute.Name.ToLower() + ":" + entity.Value.ToLower();
            else
                return String.Empty;
        }
    }
}