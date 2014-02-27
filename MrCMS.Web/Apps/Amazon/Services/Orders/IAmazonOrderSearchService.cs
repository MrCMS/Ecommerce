using System;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using MrCMS.Entities.Indexes;
using MrCMS.Indexing.Management;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Indexing.AmazonOrderSearch;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;

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
                var q = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30,
                    new[]
                    {
                        FieldDefinition.GetFieldName<AmazonOrderAmountFieldDefinition>(),
                        FieldDefinition.GetFieldName<AmazonOrderEmailFieldDefinition>(),
                        FieldDefinition.GetFieldName<AmazonOrderIdFieldDefinition>(),
                        FieldDefinition.GetFieldName<AmazonOrderNameFieldDefinition>(),
                        FieldDefinition.GetFieldName<AmazonOrderPurchaseDateFieldDeginition>(),
                        FieldDefinition.GetFieldName<AmazonOrderStatusFieldDefinition>()
                    },
                    MrCMSApplication.Get<AdminWebpageIndexDefinition>().GetAnalyser());
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
            return new TermRangeQuery(FieldDefinition.GetFieldName<AmazonOrderPurchaseDateFieldDeginition>(),
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
                    {new TermQuery(new Term(FieldDefinition.GetFieldName<AmazonOrderStatusFieldDefinition>(), Status.ToString())), Occur.MUST}
                };
        }

        private Query GetOrderIdQuery()
        {
            return new BooleanQuery
                {
                    {new TermQuery(new Term(FieldDefinition.GetFieldName<AmazonOrderIdFieldDefinition>(), OrderId)), Occur.MUST}
                };
        }
        
        public Filter GetFilter()
        {
            return null;
        }
    }
}