using System;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class KeyEqualityComparer<T, TKey> : IEqualityComparer<T>
    {
        protected readonly Func<T, TKey> KeyExtractor;

        protected KeyEqualityComparer(Func<T, TKey> keyExtractor)
        {
            KeyExtractor = keyExtractor;
        }

        public virtual bool Equals(T x, T y)
        {
            return KeyExtractor(x).Equals(this.KeyExtractor(y));
        }

        public int GetHashCode(T obj)
        {
            return KeyExtractor(obj).GetHashCode();
        }
    }
}