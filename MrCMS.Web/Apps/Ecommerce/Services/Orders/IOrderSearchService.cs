using System;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using MrCMS.Entities.Indexes;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Indexing;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public interface IOrderSearchService
    {
        IPagedList<Entities.Orders.Order> SearchOrders(string email, string lastname, string orderid, DateTime datefrom, DateTime dateto,
            PaymentStatus paymentStatus = PaymentStatus.Pending, ShippingStatus shippingStatus = ShippingStatus.Pending, int page = 1, int pageSize = 10);
    }

    public class OrderSearchQuery
    {
        public string Email { get; set; }
        public string LastName { get; set; }
        public string OrderId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public ShippingStatus ShippingStatus { get; set; }

        public OrderSearchQuery(string email, string lastname, string orderid, DateTime datefrom, DateTime dateto,
            PaymentStatus paymentStatus = PaymentStatus.Pending, ShippingStatus shippingStatus = ShippingStatus.Pending)
        {
            Email = email;
            LastName = lastname;
            OrderId = orderid;
            DateFrom = datefrom;
            DateTo = dateto;
            PaymentStatus = paymentStatus;
            ShippingStatus = shippingStatus;
        }

        public Query GetQuery()
        {
            if (String.IsNullOrWhiteSpace(Email) && 
                String.IsNullOrWhiteSpace(LastName) && 
                String.IsNullOrWhiteSpace(OrderId) &&
                PaymentStatus == PaymentStatus.Pending &&
                ShippingStatus == ShippingStatus.Pending)
                return new MatchAllDocsQuery();

            var booleanQuery = new BooleanQuery();
            if (!String.IsNullOrWhiteSpace(Email))
                booleanQuery.Add(GetEmailQuery(), Occur.MUST);
            if (!String.IsNullOrWhiteSpace(LastName))
                booleanQuery.Add(GetLastNameQuery(), Occur.MUST);
            if (!String.IsNullOrWhiteSpace(OrderId))
                booleanQuery.Add(GetOrderIdQuery(), Occur.MUST);
            booleanQuery.Add(GetPaymentStatusQuery(), Occur.MUST);
            booleanQuery.Add(GetShippingStatusQuery(), Occur.MUST);
            return booleanQuery;
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

        private Query GetLastNameQuery()
        {
            return new BooleanQuery
                {
                    {new TermQuery(new Term(OrderSearchIndex.LastName.FieldName, LastName)), Occur.MUST}
                };
        }

        private Query GetEmailQuery()
        {
            return new BooleanQuery
                { 
                    {new TermQuery(new Term(OrderSearchIndex.Email.FieldName,Email)), Occur.MUST}
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