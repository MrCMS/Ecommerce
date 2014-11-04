using System;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using MrCMS.Indexing.Management;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Indexing;
using MrCMS.Web.Apps.Ecommerce.Indexing.OrderSearchIndexDefinitions;
using MrCMS.Web.Apps.Ecommerce.Indexing.ProductSearchFieldDefinitions;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public interface IOrderSearchService
    {
        IPagedList<Order> SearchOrders(OrderSearchModel model, int page = 1, int pageSize = 10);
    }

    public class OrderSearchQuery
    {
        public string SearchText { get; set; }
        public string OrderId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public ShippingStatus? ShippingStatus { get; set; }
        public string SalesChannel { get; set; }
        public double? OrderTotalFrom { get; set; }
        public double? OrderTotalTo { get; set; }

        public OrderSearchQuery(OrderSearchModel orderSearchModel)
        {
            SearchText = orderSearchModel.SearchText;
            OrderId = orderSearchModel.OrderId;
            DateFrom = orderSearchModel.DateFrom;
            DateTo = orderSearchModel.DateTo;
            PaymentStatus = orderSearchModel.PaymentStatus;
            ShippingStatus = orderSearchModel.ShippingStatus;
            SalesChannel = orderSearchModel.SalesChannel;
            OrderTotalFrom = orderSearchModel.OrderTotalFrom;
            OrderTotalTo = orderSearchModel.OrderTotalTo;
        }

        public Query GetQuery()
        {
            if (String.IsNullOrWhiteSpace(SearchText) && String.IsNullOrWhiteSpace(OrderId) && PaymentStatus == null &&
                ShippingStatus == null && !DateFrom.HasValue && !DateTo.HasValue && string.IsNullOrWhiteSpace(SalesChannel) && !OrderTotalFrom.HasValue && !OrderTotalTo.HasValue)
                return new MatchAllDocsQuery();

            var booleanQuery = new BooleanQuery();
            if (!String.IsNullOrWhiteSpace(SearchText))
            {
                var fuzzySearchTerm = MakeFuzzy(SearchText);
                var q = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30,
                    new[]
                    {
                        FieldDefinition.GetFieldName<OrderSearchCreatedOnDefinition>(),
                        FieldDefinition.GetFieldName<OrderSearchEmaillDefinition>(),
                        FieldDefinition.GetFieldName<OrderSearchLastnamelDefinition>(),
                        FieldDefinition.GetFieldName<OrderSearchPaymentStatusDefinition>(),
                        FieldDefinition.GetFieldName<OrderSearchShippingStatusDefinition>(),
                        FieldDefinition.GetFieldName<OrderSearchTotalDefinition>(),
                    },
                    MrCMSApplication.Get<OrderSearchIndex>().GetAnalyser());
                var query = q.Parse(fuzzySearchTerm);
                booleanQuery.Add(query, Occur.SHOULD);
            }
            if (DateFrom.HasValue || DateTo.HasValue)
                booleanQuery.Add(GetDateQuery(), Occur.MUST);
            if (!String.IsNullOrWhiteSpace(OrderId))
                booleanQuery.Add(GetOrderIdQuery(), Occur.MUST);
            if (PaymentStatus != null)
                booleanQuery.Add(GetPaymentStatusQuery(), Occur.MUST);
            if (ShippingStatus != null)
                booleanQuery.Add(GetShippingStatusQuery(), Occur.MUST);
            if (!string.IsNullOrWhiteSpace(SalesChannel))
                booleanQuery.Add(GetSalesChannelQuery(), Occur.MUST);
            if (OrderTotalFrom > 0 || OrderTotalTo.HasValue)
                booleanQuery.Add(GetPriceRangeQuery(), Occur.MUST);

            return booleanQuery;
        }

        private Query GetPriceRangeQuery()
        {
            var booleanQuery = new BooleanQuery
            {
                {
                    NumericRangeQuery.NewDoubleRange(
                        FieldDefinition.GetFieldName<OrderSearchTotalDefinition>(),
                        OrderTotalFrom, OrderTotalTo, true, OrderTotalTo.HasValue),
                    Occur.MUST
                }
            };
            return booleanQuery;
        }


        private Query GetDateQuery()
        {
            return new TermRangeQuery(FieldDefinition.GetFieldName<OrderSearchCreatedOnDefinition>(),
                                      DateFrom.HasValue
                                          ? DateTools.DateToString(DateFrom.Value, DateTools.Resolution.SECOND)
                                          : null,
                                      DateTo.HasValue
                                          ? DateTools.DateToString(DateTo.Value, DateTools.Resolution.SECOND)
                                          : null, DateFrom.HasValue, DateTo.HasValue);
        }

        private string MakeFuzzy(string keywords)
        {
            var split = keywords.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", split.Select(s => s + "~"));
        }

        private Query GetSalesChannelQuery()
        {
            return new BooleanQuery
                {
                    {new TermQuery(new Term(FieldDefinition.GetFieldName<OrderSearchSalesChannelDefinition>(), SalesChannel)), Occur.MUST}
                };
        }

        private Query GetShippingStatusQuery()
        {
            return new BooleanQuery
                {
                    {new TermQuery(new Term(FieldDefinition.GetFieldName<OrderSearchShippingStatusDefinition>(), ShippingStatus.ToString())), Occur.MUST}
                };
        }

        private Query GetPaymentStatusQuery()
        {
            return new BooleanQuery
                {
                    {new TermQuery(new Term(FieldDefinition.GetFieldName<OrderSearchPaymentStatusDefinition>(), PaymentStatus.ToString())), Occur.MUST}
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
            return null;
        }
    }
}