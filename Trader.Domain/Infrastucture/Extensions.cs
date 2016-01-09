
using System.Collections.Generic;
using System.Linq;
using DynamicData.Kernel;

// ReSharper disable once CheckNamespace
namespace System
{


    public static class Extensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static bool NextBoolean(this Random source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source.NextDouble() > 0.5;
        }

        public static string Pluralise(this string source, int count)
        {
            if (count == 1) return string.Format("{0} {1}", count, source);
            return string.Format("{0} {1}s", count, source); ;
        }
    }
}

namespace System.Collections.Generic
{

    public static class Extensions
    {
        
        public static string ToDelimited<T>(this IEnumerable<T> source, string delimiter=",")
        {
            if (source == null) throw new ArgumentNullException("source");
            return string.Join(delimiter, source.WithDelimiter(delimiter));

        }

        public static IEnumerable<string>  WithDelimiter<T>(this IEnumerable<T> source, string delimiter)
        {
            if (source == null) throw new ArgumentNullException("source");
            var array = source.AsArray();
            if (!array.Any()) yield return string.Empty;

            yield return array.Select(t => t.ToString()).First();

            foreach (var item in array.Skip(1))
                yield return string.Format("{0}{1}",delimiter,item);

        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");

            foreach (var item in source)
            {
                action(item);
            }
        }

    }
}
