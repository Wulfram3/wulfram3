using System.Collections.Generic;
using System.Collections;

public static class IEnumerableExtension
{
    public static List<T> ToList<T>(this IEnumerable<T> collection)
    {
        List<T> list = new List<T>();

        foreach(T entry in collection)
        {
            list.Add(entry);
        }
        return list;
    }
}
