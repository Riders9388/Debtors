using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Debtors.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            if (collection == null || collection.Count() == 0)
                return true;

            return false;
        }
    }
}
