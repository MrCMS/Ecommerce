using System;
using System.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using MrCMS.Indexing.Management;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Indexing;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Ecommerce.Indexing;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Orders
{
    public interface IAmazonOrderSearchService
    {
        IPagedList<AmazonOrder> Search(AmazonOrderSearchModel model, int page = 1, int pageSize = 10);
    }

    public class AmazonOrderSearchQuery
    {
        public string SearchText { get; set; }
        public string OrderId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public ShippingStatus? Status { get; set; }

        public AmazonOrderSearchQuery(AmazonOrderSearchModel model)
        {
            SearchText = model.SearchText;
            OrderId = model.AmazonOrderId;
            DateFrom = model.DateFrom;
            DateTo = model.DateTo;
            Status = model.ShippingStatus;
        }

        public Query GetQuery()
        {
            if (String.IsNullOrWhiteSpace(SearchText) &&  
                String.IsNullOrWhiteSpace(OrderId) &&
                Status == null)
                return new MatchAllDocsQuery();

            var booleanQuery = new BooleanQuery();

            if (!String.IsNullOrWhiteSpace(SearchText))
            {
                var fuzzySearchTerm = MakeFuzzy(SearchText);
                var q = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, FieldDefinition.GetFieldNames(AmazonOrderSearchIndex.Email, AmazonOrderSearchIndex.Name, AmazonOrderSearchIndex.AmazonOrderId), new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
                var query = q.Parse(fuzzySearchTerm);
                booleanQuery.Add(query, Occur.SHOULD);
            }

            if (!String.IsNullOrWhiteSpace(OrderId))
                booleanQuery.Add(GetOrderIdQuery(), Occur.MUST);
            if (Status != null)
                booleanQuery.Add(GetShippingStatusQuery(), Occur.MUST);
            if (DateFrom.HasValue || DateTo.HasValue)
                booleanQuery.Add(GetDateQuery(), Occur.MUST);

            return booleanQuery;
        }

        private string MakeFuzzy(string keywords)
        {
            var split = keywords.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", split.Select(s => s + "~"));
        }

        private Query GetDateQuery()
        {
            return new TermRangeQuery(AmazonOrderSearchIndex.PurchaseDate.FieldName,
                                         DateFrom.HasValue
                                             ? DateTools.DateToString(DateFrom.Value, DateTools.Resolution.SECOND)
                                             : null,
                                         DateTo.HasValue
                                             ? DateTools.DateToString(DateTo.Value, DateTools.Resolution.SECOND)
                                             : null, DateFrom.HasValue, DateTo.HasValue);
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
        
        public Filter GetFilter()
        {
            return null;
        }
    }
}