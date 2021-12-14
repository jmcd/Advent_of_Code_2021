namespace AoC;

using System.Collections.Generic;
using System.Linq;

public static class Counter
{
    public static IDictionary<T, int> ToCountDictionary<T>(this IEnumerable<T> items) where T : notnull
    {
        return items.Aggregate(new Dictionary<T, int>(), (dictionary, item) =>
        {
            if (!dictionary.TryGetValue(item, out var currentCount)) { currentCount = 0; }
            dictionary[item] = currentCount + 1;
            return dictionary;
        });
    }
}