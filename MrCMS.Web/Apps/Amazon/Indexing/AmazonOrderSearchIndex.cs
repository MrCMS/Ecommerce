using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using MrCMS.Entities.Multisite;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Website;
using NHibernate;
using System.Linq;
using MrCMS.Indexing.Utils;
using MrCMS.Helpers;
using NHibernate.Criterion;
using Version = Lucene.Net.Util.Version;
namespace MrCMS.Web.Apps.Amazon.Indexing
{
    public class AmazonOrderSearchIndex : IIndexDefinition<AmazonOrder>
    {
        public Document Convert(AmazonOrder entity)
        {
            return new Document().SetFields(Definitions, entity);
        }

        public Term GetIndex(AmazonOrder entity)
        {
            return new Term("id", entity.Id.ToString());
        }

        public AmazonOrder Convert(ISession session, Document document)
        {
            var id = document.GetValue<int>("id");
            return session.Get<AmazonOrder>(id);
        }

        public IEnumerable<AmazonOrder> Convert(ISession session, IEnumerable<Document> documents)
        {
            var ids = documents.Select(document => document.GetValue<int>("id")).ToList();
            var products =
                ids.Chunk(100)
                   .SelectMany(
                       ints => session.QueryOver<AmazonOrder>().Where(product => product.Id.IsIn(ints.ToList())).List());

            return products.OrderBy(product => ids.IndexOf(product.Id));
        }

        public string GetLocation(Site currentSite)
        {
            var location = string.Format("~/App_Data/Indexes/{0}/AmazonOrders/", currentSite.Id);
            var mapPath = CurrentRequestData.CurrentContext.Server.MapPath(location);
            return mapPath;
        }

        public Analyzer GetAnalyser()
        {
            return new StandardAnalyzer(Version.LUCENE_30);
        }

        public string IndexName { get { return "Amazon Order Search Index"; } }

        public IEnumerable<FieldDefinition<AmazonOrder>> Definitions
        {
            get
            {
                yield return Id;
                yield return AmazonOrderId;
                yield return Email;
                yield return Name;
                yield return Amount;
                yield return Status;
                yield return PurchaseDate;
            }
        }
        public static FieldDefinition<AmazonOrder> Id { get { return _id; } }
        public static FieldDefinition<AmazonOrder> AmazonOrderId { get { return _amazonOrderId; } }
        public static FieldDefinition<AmazonOrder> Email { get { return _email; } }
        public static FieldDefinition<AmazonOrder> Name { get { return _name; } }
        public static FieldDefinition<AmazonOrder> Amount { get { return _amount; } }
        public static FieldDefinition<AmazonOrder> Status { get { return _status; } }
        public static FieldDefinition<AmazonOrder> PurchaseDate { get { return _purchaseDate; } }

        private static readonly FieldDefinition<AmazonOrder> _id =
            new StringFieldDefinition<AmazonOrder>("id", item => item.Id.ToString(), Field.Store.YES,
                                         Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<AmazonOrder> _amazonOrderId =
            new StringFieldDefinition<AmazonOrder>("amazonOrderId", item => item.AmazonOrderId, Field.Store.YES,
                                         Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<AmazonOrder> _email =
            new StringFieldDefinition<AmazonOrder>("email", item => item.BuyerEmail, Field.Store.YES,
                                         Field.Index.ANALYZED);

        private static readonly FieldDefinition<AmazonOrder> _name =
           new StringFieldDefinition<AmazonOrder>("name", item => item.BuyerName, Field.Store.YES,
                                        Field.Index.ANALYZED);

        private static readonly FieldDefinition<AmazonOrder> _amount =
            new DecimalFieldDefinition<AmazonOrder>("amount", item => item.OrderTotalAmount, Field.Store.YES,
                                         Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<AmazonOrder> _status =
            new StringFieldDefinition<AmazonOrder>("status", item => item.Status.ToString(), Field.Store.YES,
                                         Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<AmazonOrder> _purchaseDate =
            new StringFieldDefinition<AmazonOrder>("purchaseDate",
                                         item =>
                                         DateTools.DateToString(item.PurchaseDate.HasValue?item.PurchaseDate.Value:item.CreatedOn,
                                                                DateTools.Resolution.SECOND), Field.Store.NO,
                                         Field.Index.NOT_ANALYZED);

    }
}