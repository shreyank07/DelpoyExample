using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyFramework.Helpers
{
    public static class SearchFilterHelpers
    {
        public static int MoveNext<T>(this ICollection<T> collection, int currentItemIndex, Func<T, bool> predicate)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));
            
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            if (currentItemIndex < 0 || currentItemIndex >= collection.Count)
                throw new ArgumentOutOfRangeException(nameof(currentItemIndex));
            
            var unmatchedResults = collection.Skip(currentItemIndex + 1).TakeWhile(item => predicate(item) == false);

            int indexOfValidResult = currentItemIndex + unmatchedResults.Count() + 1;

            if (indexOfValidResult < collection.Count)
                return indexOfValidResult;
            else
                return -1;
        }

        public static int MovePrevious<T>(this ICollection<T> collection, int currentItemIndex, Func<T, bool> predicate)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));

            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            if (currentItemIndex < 0 || currentItemIndex >= collection.Count)
                throw new ArgumentOutOfRangeException(nameof(currentItemIndex));

            var unmatchedResults = collection.Take(currentItemIndex).Reverse().TakeWhile(item => predicate(item) == false);

            int indexOfValidResult = currentItemIndex - unmatchedResults.Count();

            if (indexOfValidResult > 0)
                return indexOfValidResult;
            else
                return -1;
        }
    }
}
