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

        public int Option1ValueDisplayOrder { get; set; }
        public int Option2ValueDisplayOrder { get; set; }
        public int Option3ValueDisplayOrder { get; set; }

        public int Option1Id { get; set; }
        public int Option2Id { get; set; }
        public int Option3Id { get; set; }
    }
}