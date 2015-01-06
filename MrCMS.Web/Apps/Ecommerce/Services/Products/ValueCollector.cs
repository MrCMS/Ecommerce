using System.Collections.Generic;
using Lucene.Net.Index;
using Lucene.Net.Search;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class ValueCollector : Collector
    {
        private readonly IndexSearcher _indexSearcher;
        private readonly Dictionary<string, HashSet<string>> _values = new Dictionary<string, HashSet<string>>();

        public ValueCollector(IndexSearcher indexSearcher, params string[] fieldNames)
        {
            _indexSearcher = indexSearcher;
            foreach (var fieldName in fieldNames)
            {
                _values[fieldName] = new HashSet<string>();
            }
        }

        public override void SetScorer(Scorer scorer) { }
        public override void Collect(int doc)
        {
            var document = _indexSearcher.Doc(doc);
            foreach (var key in Values.Keys)
            {
                Values[key].AddRange(document.GetValues(key));
            }
        }

        public override void SetNextReader(IndexReader reader, int docBase) { }

        public override bool AcceptsDocsOutOfOrder { get { return true; } }

        public Dictionary<string, HashSet<string>> Values
        {
            get { return _values; }
        }
    }
}