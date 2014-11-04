using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.Products;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class OptionSearchData
    {
        public List<int> Specifications { get; set; }
        public List<OptionInfo> Options { get; set; }
    }
}