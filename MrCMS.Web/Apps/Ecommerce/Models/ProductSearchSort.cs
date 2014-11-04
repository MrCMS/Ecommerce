using System.ComponentModel;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public enum ProductSearchSort
    {
        [Description("Most Popular")]
        MostPopular = 6,
        [Description("Latest")]
        Latest = 7,
        Relevance = 1,
        [Description("Name A-Z")]
        NameAToZ = 2,
        [Description("Name Z-A")]
        NameZToA = 3,
        [Description("Price Low-High")]
        PriceLowToHigh = 4,
        [Description("Price High-Low")]
        PriceHighToLow = 5,
    }
}