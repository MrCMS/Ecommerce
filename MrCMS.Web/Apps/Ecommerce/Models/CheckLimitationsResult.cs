using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class CheckLimitationsResult
    {
        private CheckLimitationsResult()
        {
            CartItems = new HashSet<CartItemData>();
        }

        public HashSet<CartItemData> CartItems { get; set; }
        public string[] Messages { get; private set; }

        public string FormattedMessage
        {
            get { return string.Join(", ", Messages); }
        }

        public CheckLimitationsResultStatus Status { get; private set; }

        public bool Success
        {
            get { return Status == CheckLimitationsResultStatus.Success; }
        }

        public static CheckLimitationsResult NeverValid(params string[] messages)
        {
            return new CheckLimitationsResult
            {
                Status = CheckLimitationsResultStatus.NeverValid,
                Messages = messages
            };
        }

        public static CheckLimitationsResult CurrentlyInvalid(params string[] messages)
        {
            return new CheckLimitationsResult
            {
                Status = CheckLimitationsResultStatus.CurrentlyInvalid,
                Messages = messages
            };
        }

        public static CheckLimitationsResult Successful(IEnumerable<CartItemData> applicableItems,
            params string[] messages)
        {
            var checkLimitationsResult = new CheckLimitationsResult
            {
                Status = CheckLimitationsResultStatus.Success,
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
                Status = results.Any(x => x.Status == CheckLimitationsResultStatus.NeverValid)
                    ? CheckLimitationsResultStatus.NeverValid
                    : results.Any(x => x.Status == CheckLimitationsResultStatus.CurrentlyInvalid)
                        ? CheckLimitationsResultStatus.CurrentlyInvalid
                        : CheckLimitationsResultStatus.Success,
                Messages = results.SelectMany(x => x.Messages).ToArray(),
                CartItems = results.SelectMany(x => x.CartItems).Distinct().ToHashSet()
            };
        }
    }
}