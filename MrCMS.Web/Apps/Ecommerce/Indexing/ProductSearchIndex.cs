using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Util;
using MrCMS.Entities.Multisite;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website;
using NHibernate;
using System.Linq;
using MrCMS.Indexing.Utils;
using MrCMS.Helpers;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Indexing
{
    public class ProductSearchIndex : IIndexDefinition<Product>
    {
        public Document Convert(Product entity)
        {
            var document = new Document();
            document.AddField("id", entity.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED)
                    .AddField("name", entity.Name, Field.Store.YES, Field.Index.ANALYZED)
                    .AddField("bodycontent", entity.BodyContent, Field.Store.NO, Field.Index.ANALYZED)
                    .AddField("metatitle", entity.MetaTitle, Field.Store.NO, Field.Index.ANALYZED)
                    .AddField("metakeywords", entity.MetaKeywords, Field.Store.NO, Field.Index.ANALYZED)
                    .AddField("metadescription", entity.MetaDescription, Field.Store.NO, Field.Index.ANALYZED)
                    .AddField("urlsegment", entity.UrlSegment, Field.Store.NO, Field.Index.ANALYZED)
                    .AddField("publishOn",
                              entity.PublishOn.HasValue
                                  ? DateTools.DateToString(entity.PublishOn.Value, DateTools.Resolution.SECOND)
                                  : null, Field.Store.NO, Field.Index.NOT_ANALYZED);
            return document;
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

        public string Name { get { return "Product Search Index"; }}
    }
}