using System;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Indexing;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Orders
{
    public interface IAmazonOrderSearchService
    {
        IPagedList<AmazonOrder> Search(string email, string lastname, string orderid, DateTime datefrom, DateTime dateto,
            ShippingStatus shippingStatus = ShippingStatus.Pending, int page = 1, int pageSize = 10);
    }

    public class AmazonOrderSearchQuery
    {
        public string Email { get; set; }
        public string LastName { get; set; }
        public string OrderId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public ShippingStatus Status { get; set; }

        public AmazonOrderSearchQuery(string email, string lastname, string orderid, DateTime datefrom, DateTime dateto,
            ShippingStatus shippingStatus = ShippingStatus.Pending)
        {
            Email = email;
            LastName = lastname;
            OrderId = orderid;
            DateFrom = datefrom;
            DateTo = dateto;
            Status = shippingStatus;
        }

        public Query GetQuery()
        {
            if (String.IsNullOrWhiteSpace(Email) && 
                String.IsNullOrWhiteSpace(LastName) && 
                String.IsNullOrWhiteSpace(OrderId) &&
                Status == ShippingStatus.Pending)
                return new MatchAllDocsQuery();

            var booleanQuery = new BooleanQuery();
            if (!String.IsNullOrWhiteSpace(Email))
                booleanQuery.Add(GetEmailQuery(), Occur.MUST);
            if (!String.IsNullOrWhiteSpace(LastName))
                booleanQuery.Add(GetLastNameQuery(), Occur.MUST);
            if (!String.IsNullOrWhiteSpace(OrderId))
                booleanQuery.Add(GetOrderIdQuery(), Occur.MUST);
            booleanQuery.Add(GetShippingStatusQuery(), Occur.MUST);
            return booleanQuery;
        }

        private Query GetShippingStatusQuery()
        {
            return new BooleanQuery
                {
                    {new TermQuery(new Term(AmazonOrderSearchIndex.Status.FieldName, Status.ToString())), Occur.MUST}
                };
        }

        private Query GetOrderIdQuery()
        {
            return new BooleanQuery
                {
                    {new TermQuery(new Term(AmazonOrderSearchIndex.AmazonOrderId.FieldName, OrderId)), Occur.MUST}
                };
        }

        private Query GetLastNameQuery()
        {
            return new BooleanQuery
                {
                    {new TermQuery(new Term(AmazonOrderSearchIndex.Name.FieldName, LastName)), Occur.MUST}
                };
        }

        private Query GetEmailQuery()
        {
            return new BooleanQuery
                { 
                    {new TermQuery(new Term(AmazonOrderSearchIndex.Email.FieldName,Email)), Occur.MUST}
                };
        }

        public Filter GetFilter()
        {
            var dateFrom = DateTools.DateToString(DateFrom, DateTools.Resolution.SECOND);
            var dateTo = DateTools.DateToString(DateTo, DateTools.Resolution.SECOND);
            return FieldCacheRangeFilter.NewStringRange(AmazonOrderSearchIndex.PurchaseDate.FieldName, dateFrom,
                                                              dateTo, false, true);
        }
    }
}