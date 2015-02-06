using System.Collections.Generic;
using System.Collections.Specialized;
using NHibernate.Mapping;

namespace MrCMS.Web.Apps.Ecommerce.Payment.CharityClear.Models
{
    public class CharityClearPostModel
    {
        public SortedDictionary<string, string> Fields { get; set; }
    }
}