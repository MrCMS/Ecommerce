using System;
using System.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using MrCMS.Indexing.Management;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Indexing;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public interface IOrderSearchService
    {
        IPagedList<Entities.Orders.Order> SearchOrders(string searchText, string orderid, DateTime datefrom, DateTime dateto,
            PaymentStatus? paymentStatus, ShippingStatus? shippingStatus, int page = 1, int pageSize = 10);
    }

    public class OrderSearchQuery
    {
        public string SearchText { get; set; }
        public string OrderId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public ShippingStatus? ShippingStatus { get; set; }

        public OrderSearchQuery(string searchText, string orderid, DateTime datefrom, DateTime dateto,
            PaymentStatus? paymentStatus, ShippingStatus? shippingStatus)
        {
            SearchText = searchText;
            OrderId = orderid;
            DateFrom = datefrom;
            DateTo = dateto;
            PaymentStatus = paymentStatus;
            ShippingStatus = shippingStatus;
        }

        public Query GetQuery()
        {
            if (String.IsNullOrWhiteSpace(SearchText) && 
                String.IsNullOrWhiteSpace(OrderId) &&
                PaymentStatus == null &&
                ShippingStatus == null)
                return new MatchAllDocsQuery();

            var booleanQuery = new BooleanQuery();
            if (!String.IsNullOrWhiteSpace(SearchText))
            {
                var fuzzySearchTerm = MakeFuzzy(SearchText);
                var q = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, FieldDefinition.GetFieldNames(OrderSearchIndex.Email, OrderSearchIndex.LastName), new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
                var query = q.Parse(fuzzySearchTerm);
                booleanQuery.Add(query, Occur.SHOULD);
            }
            if (!String.IsNullOrWhiteSpace(OrderId))
                booleanQuery.Add(GetOrderIdQuery(), Occur.MUST);
            if (PaymentStatus != null)
                booleanQuery.Add(GetPaymentStatusQuery(), Occur.MUST);
            if (ShippingStatus != null)
                booleanQuery.Add(GetShippingStatusQuery(), Occur.MUST);
            return booleanQuery;
        }

        private string MakeFuzzy(string keywords)
        {
            var split = keywords.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", split.Select(s => s + "~"));
        }

        private Query GetShippingStatusQuery()
        {
            return new BooleanQuery
                {
                    {new TermQuery(new Term(OrderSearchIndex.ShippingStatus.FieldName, ShippingStatus.ToString())), Occur.MUST}
                };
        }

        private Query GetPaymentStatusQuery()
        {
            return new BooleanQuery
                {
                    {new TermQuery(new Term(OrderSearchIndex.PaymentStatus.FieldName, PaymentStatus.ToString())), Occur.MUST}
                };
        }

        private Query GetOrderIdQuery()
        {
            return new BooleanQuery
                {
                    {new TermQuery(new Term(OrderSearchIndex.Id.FieldName, OrderId)), Occur.MUST}
                };
        }

        public Filter GetFilter()
        {
            var dateFrom = DateTools.DateToString(DateFrom, DateTools.Resolution.SECOND);
            var dateTo = DateTools.DateToString(DateTo, DateTools.Resolution.SECOND);
            return FieldCacheRangeFilter.NewStringRange(OrderSearchIndex.CreatedOn.FieldName, dateFrom,
                                                              dateTo, false, true);
        }
    }
}