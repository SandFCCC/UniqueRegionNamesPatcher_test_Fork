using Noggog;
using System.Collections.Generic;

namespace UniqueRegionNamesPatcher.Extensions
{
    public static class ListExtensions
    {
        public static void AddOrReplace<T>(this IList<T> list, T obj)
        {
            var i = list.IndexOf(obj);
            if (!i.Equals(-1))
                list[i] = obj;
            else
                list.Add(obj);
        }
        public static void AddOrReplaceRange<T>(this IList<T> list, IReadOnlyList<T> other) => other.ForEach(o => list.AddOrReplace(o));
    }
}
