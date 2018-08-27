using System;
using System.Collections.Generic;

namespace FISE_API
{
    public static class ExtensionMethods
    {

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.DistinctBy(keySelector, null);
        }


        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            return DistinctByImpl(source, keySelector, comparer);
        }

        private static IEnumerable<TSource> DistinctByImpl<TSource, TKey>(IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
#if !NO_HASHSET
            var knownKeys = new HashSet<TKey>(comparer);
            foreach (var element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
#else
                        //
                        // On platforms where LINQ is available but no HashSet<T>
                        // (like on Silverlight), implement this operator using 
                        // existing LINQ operators. Using GroupBy is slightly less
                        // efficient since it has do all the grouping work before
                        // it can start to yield any one element from the source.
                        //

                        return source.GroupBy(keySelector, comparer).Select(g => g.First());
#endif
        }


        public static int? ToNullableInt32(this string s)
        {
            int i;
            if(Int32.TryParse(s,out i))
            {
                return i;
            }
            return null;
        }

        public static int? NullableParse(this int i, string str)
        {
            return null;
        }
    }
}