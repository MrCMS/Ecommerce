using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class CheckLimitationsResult
    {
        private CheckLimitationsResult()
        {
            CartItems = new List<CartItem>();
        }

        public List<CartItem> CartItems { get; set; }
        public string[] Messages { get; private set; }

        public bool Success { get; private set; }

        public static CheckLimitationsResult Failure(params string[] messages)
        {
            return new CheckLimitationsResult
            {
                Success = false,
                Messages = messages
            };
        }

        public static CheckLimitationsResult Successful(IEnumerable<CartItem> applicableItems, params string[] messages)
        {
            var checkLimitationsResult = new CheckLimitationsResult
            {
                Success = true,
                Messages = messages
            };
            if (applicableItems != null)
                checkLimitationsResult.CartItems.AddRange(applicableItems);
            return checkLimitationsResult;
        }

        public static CheckLimitationsResult Combine(params CheckLimitationsResult[] results)
        {
            return new CheckLimitationsResult
            {
                Success = results.All(x => x.Success),
                Messages = results.SelectMany(x => x.Messages).ToArray(),
                CartItems = results.SelectMany(x => x.CartItems).Distinct().ToList()
            };
        }
    }
}