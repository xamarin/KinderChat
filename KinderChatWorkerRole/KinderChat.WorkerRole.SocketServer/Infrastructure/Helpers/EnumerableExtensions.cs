using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace KinderChat.WorkerRole.SocketServer.Infrastructure.Helpers
{
    public static class EnumerableExtensions
    {
        public static void ChangeListFromDictionary<TKey, TValue>(this ConcurrentDictionary<TKey, List<TValue>> dictionary,
            TKey key, Action<List<TValue>> actionOverList)
        {
            List<TValue> values;
            if (dictionary.TryGetValue(key, out values))
            {
                lock (values)
                {
                    actionOverList(values);
                }
            }
            else
            {
                var list = new List<TValue>();
                actionOverList(list);
                dictionary[key] = list;
            }
        }

        public static bool IsNullOrEmpty(this IEnumerable enumerable)
        {
            if (enumerable == null)
                return true;

            foreach (var item in enumerable)
            {
                return false;
            }
            return true;
        }

        public static bool AddIfUnique<T, TProp>(this List<T> list, T value, Func<T, TProp> uniqueChecker)
        {
            var prop = uniqueChecker(value);
            if (list.Any(item => uniqueChecker(item).Equals(prop)))
            {
                return false;
            }
            list.Add(value);
            return true;
        }

        public static void FindIntersectionAndDifference<TValue>(this IEnumerable<TValue> set1, IEnumerable<TValue> set2,
            out List<TValue> intersection, out List<TValue> differenceFromSet1, out List<TValue> differenceFromSet2)
        {
            intersection = new List<TValue>();
            differenceFromSet1 = new List<TValue>();
            differenceFromSet2 = new List<TValue>();
            var hashSet2 = new HashSet<TValue>(set2);
            foreach (var valueFromSet1 in set1)
            {
                if (hashSet2.Remove(valueFromSet1))
                {
                    intersection.Add(valueFromSet1);
                }
                else
                {
                    differenceFromSet1.Add(valueFromSet1);
                }
            }
            foreach (var valueFromSet2 in hashSet2)
            {
                differenceFromSet2.Add(valueFromSet2);
            }
        }

        public static void FindIntersectionAndDifference<TValue1, TValue2, TResult>(this IEnumerable<TValue1> set1, IEnumerable<TValue2> set2,
            Func<TValue1, TResult> resultGetterForSet1, Func<TValue2, TResult> resultGetterForSet2,
            out List<TResult> intersection, out List<TResult> differenceFromSet1, out List<TResult> differenceFromSet2)
        {
            intersection = new List<TResult>();
            differenceFromSet1 = new List<TResult>();
            differenceFromSet2 = new List<TResult>();
            var hashSet2 = set2.ToDictionary(resultGetterForSet2, v => v);
            foreach (var valueFromSet1 in set1)
            {
                var value1 = resultGetterForSet1(valueFromSet1);
                if (hashSet2.Remove(value1))
                {
                    intersection.Add(value1);
                }
                else
                {
                    differenceFromSet1.Add(value1);
                }
            }
            foreach (var valueFromSet2 in hashSet2)
            {
                differenceFromSet2.Add(valueFromSet2.Key);
            }
        }
    }
}
