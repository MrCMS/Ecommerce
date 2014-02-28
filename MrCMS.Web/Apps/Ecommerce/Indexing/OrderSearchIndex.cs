using System.Collections.Generic;
using System.Linq;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Indexing
{
    public class OrderSearchIndex : IndexDefinition<Order>
    {
        private readonly HashSet<IFieldDefinition<OrderSearchIndex, Order>> _definitions;

        public OrderSearchIndex(ISession session, IEnumerable<IFieldDefinition<OrderSearchIndex, Order>> definitions)
            : base(session)
        {
            _definitions = new HashSet<IFieldDefinition<OrderSearchIndex, Order>>(definitions);
        }

        public override IEnumerable<FieldDefinition<Order>> Definitions
        {
            get { return _definitions.Select(definition => definition.GetDefinition); }
        }

        public override IEnumerable<string> FieldNames
        {
            get { return _definitions.Select(definition => definition.Name); }
        }

        public override string IndexFolderName
        {
            get { return "Orders"; }
        }

        public override string IndexName
        {
            get { return "Order Search Index"; }
        }

        public override IEnumerable<IFieldDefinitionInfo> DefinitionInfos
        {
            get { return _definitions; }
        }
    }
}

//    public static FieldDefinition<Order> Id
    //    {
    //        get { return _id; }
    //    }

    //    public static FieldDefinition<Order> Email
    //    {
    //        get { return _email; }
    //    }

    //    public static FieldDefinition<Order> LastName
    //    {
    //        get { return _lastName; }
    //    }

    //    public static FieldDefinition<Order> SalesChannel
    //    {
    //        get { return _salesChannel; }
    //    }

    //    public static FieldDefinition<Order> Amount
    //    {
    //        get { return _amount; }
    //    }

    //    public static FieldDefinition<Order> PaymentStatus
    //    {
    //        get { return _paymentStatus; }
    //    }

    //    public static FieldDefinition<Order> ShippingStatus
    //    {
    //        get { return _shippingStatus; }
    //    }

    //    public static FieldDefinition<Order> CreatedOn
    //    {
    //        get { return _createdOn; }
    //    }

    //    public Document Convert(Order entity)
    //    {
    //        return new Document().SetFields(Definitions, entity);
    //    }

    //    public Term GetIndex(Order entity)
    //    {
    //        return new Term("id", entity.Id.ToString());
    //    }

    //    public Order Convert(ISession session, Document document)
    //    {
    //        var id = document.GetValue<int>("id");
    //        return session.Get<Order>(id);
    //    }

    //    public IEnumerable<Order> Convert(ISession session, IEnumerable<Document> documents)
    //    {
    //        List<int> ids = documents.Select(document => document.GetValue<int>("id")).ToList();
    //        IEnumerable<Order> products =
    //            ids.Chunk(100)
    //               .SelectMany(
    //                   ints =>
    //                   session.QueryOver<Order>()
    //                          .Where(product => RestrictionExtensions.IsIn(product.Id, ints.ToList()))
    //                          .List());

    //        return products.OrderBy(product => ids.IndexOf(product.Id));
    //    }

    //    public string GetLocation(Site currentSite)
    //    {
    //        string location = string.Format("~/App_Data/Indexes/{0}/Orders/", currentSite.Id);
    //        string mapPath = CurrentRequestData.CurrentContext.Server.MapPath(location);
    //        return mapPath;
    //    }

    //    public Analyzer GetAnalyser()
    //    {
    //        return new StandardAnalyzer(Version.LUCENE_30);
    //    }

    //    public string IndexName
    //    {
    //        get { return "Order Search Index"; }
    //    }

    //    public string IndexFolderName
    //    {
    //        get { return "Orders"; }
    //    }

    //    public IEnumerable<FieldDefinition<Order>> Definitions
    //    {
    //        get
    //        {
    //            yield return Id;
    //            yield return Email;
    //            yield return LastName;
    //            yield return SalesChannel;
    //            yield return Amount;
    //            yield return PaymentStatus;
    //            yield return ShippingStatus;
    //            yield return CreatedOn;
    //        }
    //    }

    //    private static readonly FieldDefinition<Order> _id =
    //        new StringFieldDefinition<Order>("id", item => item.Id.ToString(), Field.Store.YES,
    //                                         Field.Index.NOT_ANALYZED);

    //    private static readonly FieldDefinition<Order> _salesChannel =
    //        new StringFieldDefinition<Order>("saleschannel", item => item.SalesChannel, Field.Store.NO,
    //                                         Field.Index.NOT_ANALYZED);

    //    private static readonly FieldDefinition<Order> _email =
    //        new StringFieldDefinition<Order>("email", item => item.OrderEmail, Field.Store.YES,
    //                                         Field.Index.ANALYZED);

    //    private static readonly FieldDefinition<Order> _lastName =
    //        new StringFieldDefinition<Order>("lastName",
    //                                         item =>
    //                                         item.ShippingAddress != null ? item.ShippingAddress.Name : String.Empty,
    //                                         Field.Store.YES,
    //                                         Field.Index.ANALYZED);

    //    private static readonly FieldDefinition<Order> _amount =
    //        new DecimalFieldDefinition<Order>("amount", item => item.Total, Field.Store.NO,
    //                                          Field.Index.NOT_ANALYZED);

    //    private static readonly FieldDefinition<Order> _paymentStatus =
    //        new StringFieldDefinition<Order>("paymentStatus", item => item.PaymentStatus.ToString(), Field.Store.NO,
    //                                         Field.Index.NOT_ANALYZED);

    //    private static readonly FieldDefinition<Order> _shippingStatus =
    //        new StringFieldDefinition<Order>("shippingStatus", item => item.ShippingStatus.ToString(), Field.Store.NO,
    //                                         Field.Index.NOT_ANALYZED);

    //    private static readonly FieldDefinition<Order> _createdOn =
    //        new StringFieldDefinition<Order>("createdOn",
    //                                         item =>
    //                                         DateTools.DateToString(item.CreatedOn,
    //                                                                DateTools.Resolution.SECOND), Field.Store.NO,
    //                                         Field.Index.NOT_ANALYZED);
    //}
