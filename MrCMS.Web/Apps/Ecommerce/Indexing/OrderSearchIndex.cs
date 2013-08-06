using System;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using MrCMS.Entities.Multisite;
using MrCMS.Indexing.Management;
using MrCMS.Website;
using NHibernate;
using System.Linq;
using MrCMS.Indexing.Utils;
using MrCMS.Helpers;
using NHibernate.Criterion;
using Version = Lucene.Net.Util.Version;
namespace MrCMS.Web.Apps.Ecommerce.Indexing
{
    public class OrderSearchIndex : IIndexDefinition<Entities.Orders.Order>
    {
        public Document Convert(Entities.Orders.Order entity)
        {
            return new Document().SetFields(Definitions, entity);
        }

        public Term GetIndex(Entities.Orders.Order entity)
        {
            return new Term("id", entity.Id.ToString());
        }

        public Entities.Orders.Order Convert(ISession session, Document document)
        {
            var id = document.GetValue<int>("id");
            return session.Get<Entities.Orders.Order>(id);
        }

        public IEnumerable<Entities.Orders.Order> Convert(ISession session, IEnumerable<Document> documents)
        {
            var ids = documents.Select(document => document.GetValue<int>("id")).ToList();
            var products =
                ids.Chunk(100)
                   .SelectMany(
                       ints => session.QueryOver<Entities.Orders.Order>().Where(product => product.Id.IsIn(ints.ToList())).List());

            return products.OrderBy(product => ids.IndexOf(product.Id));
        }

        public string GetLocation(Site currentSite)
        {
            var location = string.Format("~/App_Data/Indexes/{0}/Orders/", currentSite.Id);
            var mapPath = CurrentRequestData.CurrentContext.Server.MapPath(location);
            return mapPath;
        }

        public Analyzer GetAnalyser()
        {
            return new StandardAnalyzer(Version.LUCENE_30);
        }

        public string IndexName { get { return "Order Search Index"; } }

        public IEnumerable<FieldDefinition<Entities.Orders.Order>> Definitions
        {
            get
            {
                yield return Id;
                yield return Email;
                yield return LastName;
                yield return Amount;
                yield return PaymentStatus;
                yield return ShippingStatus;
                yield return CreatedOn;
            }
        }
        public static FieldDefinition<Entities.Orders.Order> Id { get { return _id; } }
        public static FieldDefinition<Entities.Orders.Order> Email { get { return _email; } }
        public static FieldDefinition<Entities.Orders.Order> LastName { get { return _lastName; } }
        public static FieldDefinition<Entities.Orders.Order> Amount { get { return _amount; } }
        public static FieldDefinition<Entities.Orders.Order> PaymentStatus { get { return _paymentStatus; } }
        public static FieldDefinition<Entities.Orders.Order> ShippingStatus { get { return _shippingStatus; } }
        public static FieldDefinition<Entities.Orders.Order> CreatedOn { get { return _createdOn; } }

        private static readonly FieldDefinition<Entities.Orders.Order> _id =
            new StringFieldDefinition<Entities.Orders.Order>("id", item => item.Id.ToString(), Field.Store.YES,
                                         Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Entities.Orders.Order> _email =
            new StringFieldDefinition<Entities.Orders.Order>("email", item => item.OrderEmail, Field.Store.YES,
                                         Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Entities.Orders.Order> _lastName =
           new StringFieldDefinition<Entities.Orders.Order>("lastName", item => item.User != null ? item.User.LastName : String.Empty, Field.Store.YES,
                                        Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Entities.Orders.Order> _amount =
            new DecimalFieldDefinition<Entities.Orders.Order>("amount", item => item.Total, Field.Store.YES,
                                         Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Entities.Orders.Order> _paymentStatus =
            new StringFieldDefinition<Entities.Orders.Order>("paymentStatus", item => item.PaymentStatus.ToString(), Field.Store.YES,
                                         Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Entities.Orders.Order> _shippingStatus =
            new StringFieldDefinition<Entities.Orders.Order>("shippingStatus", item => item.ShippingStatus.ToString(), Field.Store.YES,
                                         Field.Index.NOT_ANALYZED);

        private static readonly FieldDefinition<Entities.Orders.Order> _createdOn =
            new StringFieldDefinition<Entities.Orders.Order>("createdOn",
                                         item =>
                                         DateTools.DateToString(item.CreatedOn,
                                                                DateTools.Resolution.SECOND), Field.Store.NO,
                                         Field.Index.NOT_ANALYZED);

    }
}