using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class MakeMultivariantModel
    {
        public int ProductId { get; set; }
        [DisplayName("Option 1")]
        public string Option1 { get; set; }
        [DisplayName("Option 1 Value")]
        public string Option1Value { get; set; }
        [DisplayName("Option 2")]
        public string Option2 { get; set; }
        [DisplayName("Option 2 Value")]
        public string Option2Value { get; set; }
        [DisplayName("Option 3")]
        public string Option3 { get; set; }
        [DisplayName("Option 3 Value")]
        public string Option3Value { get; set; }
    }
}