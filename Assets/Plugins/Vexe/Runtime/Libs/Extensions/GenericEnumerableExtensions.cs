using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Vexe.Runtime.Extensions
{
	public static class GenericEnumerableExtensions
	{
        public static bool AnyIs<T>(this IEnumerable<object> items) where T : class
        {
            return items.Any(x => x is T);
        }

		public static bool IsUnique<T>(this T[] array)
		{
			for (int i = 0; i < array.Length - 1; i++)
				for (int j = i + 1; j < array.Length; j++)
					if (array[i].GenericEquals(array[j]))
						return false;
			return true;
		}

		public static bool IsUnique<T>(this List<T> list)
		{
			for (int i = 0; i < list.Count - 1; i++)
				for (int j = i + 1; j < list.Count; j++)
					if (list[i].GenericEquals(list[j]))
						return false;
			return true;
		}

		public static bool EqualElements<T>(this IList<T> source, IList<T> other)
		{
			if (source.Count != other.Count)
				return false;

			for (int i = 0; i < source.Count; i++)
			{
				if (!source[i].GenericEquals(other[i]))
					return false;
			}

			return true;
		}

		public static IEnumerable<T> OfType<T>(this IEnumerable<T> items, Type ofType)
		{
			foreach(var item in items)
			{
				if (item != null && ofType.IsAssignableFrom(item.GetType()))
				{
					yield return item;
				}
			}
		}

		public static int IndexOfZeroIfNotFound<T>(this IEnumerable<T> list, T value)
		{
			int found = list.IndexOf(value);
			return found == -1 ? 0 : found;
		}

		public static int IndexOfZeroIfNotFound<T>(this IEnumerable<T> list, Predicate<T> match)
		{
			int found = list.IndexOf(match);
			return found == -1 ? 0 : found;
		}

		public static void AdjustSize<T>(this IList<T> list, int newSize, Action<int> remove, Action add)
		{
			int dif = newSize - list.Count;
			if (dif > 0)
			{
				for (int i = 0; i < dif; i++)
					add();
			}
			else
			{
				for (int i = list.Count - 1; i >= newSize; i--)
					remove(i);
			}
		}

		/// <summary>
		/// Adds the speicfied value to the list if it wasn't already contained
		/// Returns true if the item was added
		/// </summary>
		public static bool AddIfNotExists<T>(this IList<T> list, T value)
		{
			if (list.Contains(value))
				return false;

			list.Add(value);
			return true;
		}

		/// <summary>
		/// Adds the specified value to this list if the value wasn't null
		/// Throws a NullReferenceException with the specified msg if throwNull is true with the
		/// Returns true if the value is added
		/// </summary>
		public static bool AddIfNotNull<T>(this IList<T> list, T value, bool throwNull, string msg)
		{
			if (value == null || value.Equals(null))
			{
				if (throwNull)
					throw new NullReferenceException(msg);
				return false;
			}
			list.Add(value);
			return true;
		}

		/// <summary>
		/// Adds the specified value to this list if the value wasn't null
		/// Doesn't throw exception if the value was null
		/// Returns true if the value is added
		/// </summary>
		public static bool AddIfNotNull<T>(this IList<T> list, T value)
		{
			return AddIfNotNull(list, value, false, null);
		}

		public static IEnumerable<T> Uniqify<T>(this IList<T> list, string postfix, Func<T, string> selector, Action<IList<T>, int, string> set)
		{
			var regex = new Regex(@"\$");
			IEnumerable<int[]> duplicatesIndicesLists = list.GetDuplicates(selector).Select(d => d.Value);
			foreach (var dupList in duplicatesIndicesLists)
			{
				for (int i = 0; i < dupList.Length; i++)
				{
					int idx = dupList[i];
					string item = selector(list[idx]);
					string result = item + regex.Replace(postfix, (i + 1).ToString());
					set(list, idx, result);
				}
			}
			return list;
		}

		/// <summary>
		/// Given a list of { "X", "Y", "Y", "Z", "X" } and postfix of "_$" returns { "X_1", "Y_1", "Y_2", "Z", "X_2"
		/// </summary>
		public static IEnumerable<string> Uniqify(this IList<string> list, string postfix)
		{
			return Uniqify(list, postfix,
				@selector : x => x,
				@set      : (lst, idx, value) => lst[idx] = value);
		}

		/// <summary>
		/// Given a list of { "X", "Y", "Y", "Z", "X" }, returns { "X (1)", "Y (1)", "Y (2)", "Z", "X (2)"
		/// </summary>
		public static IEnumerable<string> Uniqify(this IList<string> list)
		{
			return Uniqify(list, " ($)");
		}

		/// <summary>
		/// Populates the list using the specified getter func
		/// </summary>
		public static IList<T> Populate<T>(this IList<T> list, Func<int, T> get)
		{
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = get(i);
			}
			return list;
		}

		/// <summary>
		/// Concatenates the specified elements of a string sequence, using the specified separator between each element.
		/// </summary>
		public static string JoinString<T>(this IEnumerable<T> sequence, string seperator, Func<T, string> selector)
		{
			return string.Join(seperator, sequence.Select(selector).ToArray());
		}

		/// <summary>
		/// Concatenates the specified elements of a string sequence, using the specified separator between each element.
		/// </summary>
		public static string JoinString(this IEnumerable<string> sequence, string seperator)
		{
			return string.Join(seperator, sequence.ToArray());
		}

		/// <summary>
		/// Concatenates the specified elements of a string array, using the specified separator between each element.
		/// </summary>
		public static string JoinString(this string[] sequence, string seperator)
		{
			return string.Join(seperator, sequence);
		}

		/// <summary>
		/// Concatenates the specified elements of a string array, using the specified separator between each element
		/// starting from 'start' counting up 'count'
		/// </summary>
		public static string JoinString(this string[] sequence, string seperator, int start, int count)
		{
			return string.Join(seperator, sequence, start, count);
		}

		/// <summary>
		/// Inserts the specified item to the beginning of this list
		/// </summary>
		public static void Insert<T>(this IList<T> list, T item)
		{
			list.Insert(0, item);
		}

		/// <summary>
		/// A for loop extension for sequences
		/// Starts from 0, to sequence.Count with an increment of 1
		/// Specify an act delegate that gets passed the current iterating index
		/// </summary>
		public static void For<T>(this IList<T> sequence, Action<int> act)
		{
			For(sequence, 0, i => i < sequence.Count, 1, act);
		}

		/// <summary>
		/// A for loop extension for sequences
		/// Specify the start, continuation predicate, the increment
		/// and an act delegate that gets passed the current iterating index
		/// </summary>
		public static void For<T>(this IList<T> sequence,
			int start, Func<int, bool> predicate, int increment,
			Action<int> act)
		{
			for (int i = start; predicate(i); i += increment)
			{
				act(i);
			}
		}

		/// <summary>
		/// Clears the list then adds the specified value
		/// </summary>
		public static void ClearAndAdd<T>(this IList<T> list, T value)
		{
			list.Clear();
			list.Add(value);
		}

		/// <summary>
		/// Returns the union (combines + gets rid of duplicate elements) of the input lists using the default EqualityComparer
		/// </summary>
		public static IEnumerable<T> UnionAll<T>(this IEnumerable<IEnumerable<T>> lists)
		{
			return lists.UnionAll(EqualityComparer<T>.Default);
		}

		/// <summary>
		/// Returns the union (combines + gets rid of duplicate elements) of the input lists using the specified EqualityComparer
		/// </summary>
		public static IEnumerable<T> UnionAll<T>(this IEnumerable<IEnumerable<T>> lists, IEqualityComparer<T> comparer)
		{
			return lists.SelectMany(x => x).Distinct(comparer);
		}

		/// <summary>
		/// Returns the intersection of the input lists using the default EqualityComparer
		/// </summary>
		public static List<T> IntersectAll<T>(this IEnumerable<IEnumerable<T>> lists)
		{
			return lists.IntersectAll(EqualityComparer<T>.Default);
		}

		/// <summary>
		/// Returns the intersection of the input lists using the specified EqualityComparer
		/// </summary>
		public static List<T> IntersectAll<T>(this IEnumerable<IEnumerable<T>> lists, IEqualityComparer<T> comparer)
		{
			HashSet<T> hashSet = null;
			foreach (var list in lists)
			{
				if (hashSet == null)
				{
					hashSet = new HashSet<T>(list, comparer);
				}
				else
				{
					hashSet.IntersectWith(list);
				}
			}
			return hashSet == null ? new List<T>() : hashSet.ToList();
		}

		/// <summary>
		/// Returns a new version of this list with nulls removed
		/// </summary>
		public static List<T> ClearNulls<T>(this List<T> list)
		{
			return list.Where(item => item != null && !item.Equals(null)).ToList();
		}

		/// <summary>
		/// Returns a random element from this list
		/// </summary>
		public static T RandomElement<T>(this IList<T> list)
		{
			return RandomElement(list, new Random());
		}

		/// <summary>
		/// Returns a random element from this list using the specified Random object
		/// </summary>
		public static T RandomElement<T>(this IList<T> list, Random rand)
		{
			return list[rand.Next(0, list.Count)];
		}

		/// <summary>
		/// Returns a random element from this enumerable
		/// </summary>
		public static T RandomElement<T>(this IEnumerable<T> source)
		{
			return RandomElement(source, new Random());
		}

		/// <summary>
		/// Returns a random element from this enumerable using the specified Random object
		/// Credits to Jon Skeet: http://stackoverflow.com/questions/648196/random-row-from-linq-to-sql/648240#648240
		/// </summary>
		public static T RandomElement<T>(this IEnumerable<T> source, Random rng)
		{
			T current = default(T);
			int count = 0;
			foreach (T element in source)
			{
				count++;
				if (rng.Next(count) == 0)
				{
					current = element;
				}
			}
			if (count == 0)
			{
				throw new InvalidOperationException("Sequence was empty");
			}
			return current;
		}

		/// <summary>
		/// Removes the specified items from this list
		/// </summary>
		public static void BatchRemove<T>(this IList<T> list, IEnumerable<T> items)
		{
			foreach (var item in items)
				list.Remove(item);
		}

		/// <summary>
		/// Adds a variable number of items to this list
		/// </summary>
		public static void AddMultiple<T>(this IList<T> list, params T[] items)
		{
			for (int i = 0; i < items.Length; i++)
			{
				list.Add(items[i]);
			}
		}

		/// <summary>
		/// Returns a new version of the list that doesn't include the specified value
		/// </summary>
		public static IEnumerable<T> Disinclude<T>(this IEnumerable<T> e, T value)
		{
			return e.Where(item => !item.Equals(value));
		}

		public static IEnumerable<T> Foreach<T>(this IEnumerable<T> enumerable, Action<T, int> action)
		{
			int index = 0;
			foreach (var e in enumerable)
				action(e, index++);
			return enumerable;
		}

		public static IEnumerable<T> Foreach<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
			foreach (var e in enumerable)
				action(e);
			return enumerable;
		}

		public static void SetLast<T>(this IList<T> list, T value)
		{
			list[list.LastIndex()] = value;
		}

		public static int IndexOf<T>(this IEnumerable<T> e, T item)
		{
			return e.IndexOf(x => x.Equals(item));
		}

		public static int FindIndex<T>(this T[] array, T value)
		{
			for (int i = 0; i < array.Length; i++)
				if (array[i].GenericEquals(value))
					return i;
			return -1;
		}

		public static int IndexOf<T>(this IEnumerable<T> e, Predicate<T> match)
		{
			int i = 0;
			foreach (var item in e)
			{
				if (match(item))
					return i;
				i++;
			}
			return -1;
		}

		public static bool Contains<T>(this IEnumerable<T> e, Predicate<T> match, out int index)
		{
			return (index = e.IndexOf(match)) != -1;
		}

		public static bool Contains<T>(this IEnumerable<T> e, Predicate<T> match)
		{
			int index;
			return Contains(e, match, out index);
		}

		public static bool ContainsValue<T>(this IEnumerable<T> e, T wanted, out int index)
		{
			return Contains(e, item => wanted.Equals(item), out index);
		}

		public static bool ContainsValue<T>(this IEnumerable<T> e, T wanted)
		{
			int index;
			return ContainsValue(e, wanted, out index);
		}

		// https://msmvps.com/blogs/jon_skeet/archive/2011/01/14/reimplementing-linq-to-objects-part-35-zip.aspx
		public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(
			this IEnumerable<TFirst> first,
			IEnumerable<TSecond> second,
			Func<TFirst, TSecond, TResult> resultSelector)
		{
			using (IEnumerator<TFirst> iterator1 = first.GetEnumerator())
			using (IEnumerator<TSecond> iterator2 = second.GetEnumerator())
			{
				while (iterator1.MoveNext() && iterator2.MoveNext())
				{
					yield return resultSelector(iterator1.Current, iterator2.Current);
				}
			}
		}

		public static List<T> Filter<T>(this List<T> list, Func<T, bool> filterPredicate)
		{
			return list.Where(filterPredicate).ToList();
		}

		public static IEnumerable<KeyValuePair<TSource, int[]>> GetDuplicates<TSource, TKey>(
			this IEnumerable<TSource> e,
			Func<TSource, TKey> selector)
		{
			return e.Select((value, index) => new { Index = index, Value = value })
					.GroupBy(x => selector(x.Value))
					.Select(xg => new KeyValuePair<TSource, int[]>(xg.Select(x => x.Value).First(),
																   xg.Select(x => x.Index).ToArray()))
					.Where(kv => kv.Value.Length > 1);
		}

		public static IEnumerable<KeyValuePair<T, int[]>> GetDuplicates<T>(this IEnumerable<T> e)
		{
			return GetDuplicates(e, item => item);
		}

		public static List<T> ClearAndGet<T>(this List<T> list)
		{
			list.Clear();
			return list;
		}

		public static List<T> AddAndGet<T>(this List<T> list, T item)
		{
			list.Add(item);
			return list;
		}

		public static List<T> RemoveAtAndGet<T>(this List<T> list, int atIndex)
		{
			list.RemoveAt(atIndex);
			return list;
		}

		public static T RemoveAtAndReturn<T>(this List<T> list, int atIndex)
		{
			var item = list[atIndex];
			list.RemoveAt(atIndex);
			return item;
		}

		public static List<T> InsertAndGet<T>(this List<T> list, int index, T item)
		{
			list.Insert(index, item);
			return list;
		}

		public static List<T> RemoveAllExcept<T>(this List<T> list, int except)
		{
			int index = 0;
			list.RemoveAll(value => (index++ != except));
			return list;
		}

		public static bool IsNullOrEmpty<T>(this IList<T> list)
		{
			return list == null || list.IsEmpty();
		}

		public static bool IsEmpty<T>(this IList<T> list)
		{
			return list.Count == 0;
		}

		public static int LastIndex<T>(this IList<T> list)
		{
			return list.Count - 1;
		}

		public static void AddUniqueRange<T>(this List<T> list, IEnumerable<T> collection)
		{
			list.AddRange(collection, item => !list.Contains(item));
		}

		public static void AddRange<T>(this List<T> list, IEnumerable<T> collection, Func<T, bool> predicate)
		{
			list.AddRange(collection.Where(item => predicate(item)));
		}

		public static void Shuffle<T>(this IList<T> list)
		{
			Random rnd = new Random();
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = rnd.Next(n + 1);
				list.Swap(n, k);
			}
		}

		public static void MoveElementDown<T>(this IList<T> list, int elementIndex)
		{
			int next = (elementIndex + 1) % list.Count;
			Swap(list, elementIndex, next);
		}

		public static void MoveElementUp<T>(this IList<T> list, int elementIndex)
		{
			int previous = elementIndex - 1;
			if (previous < 0) previous = list.Count - 1;
			Swap(list, elementIndex, previous);
		}

		public static void SetToDefault<T>(this IList<T> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = default(T);
			}
		}

		public static void SetToValueDisincluding(this IList<bool> list, int disinclude, bool value)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (i == disinclude) continue;
				list[i] = value;
			}
		}

		public static bool Contains<T>(this IList<T> list, T item, out int index)
		{
			index = list.IndexOf(item);
			return index != -1;
		}

		public static bool InBounds<T>(this IList<T> list, int i)
		{
			return i > -1 && i < list.Count;
		}

		public static bool IsEqualTo<T>(this IList<T> left, IList<T> right)
		{
			if (left == null)
				throw new ArgumentNullException("left");
            if (right == null)
				throw new ArgumentNullException("right");

			if (left.Count != right.Count) return false;
            for (int i = 0; i < left.Count; i++)
                if (!left[i].GenericEquals(right[i]))
                    return false;
            return true;
		}

		public static void Push<T>(this IList<T> list, T value)
		{
			list.Insert(0, value);
		}

		public static void Print<T>(this IEnumerable<T> enumerable, Action<string> output)
		{
			foreach (var item in enumerable)
			{
				output(item.ToString());
			}
		}

		public static IList<T> Swap<T>(this IList<T> list, int i, int j)
		{
			T temp = list[i];
			list[i] = list[j];
			list[j] = temp;
			return list;
		}

		public static IList<T> Shift<T>(this IList<T> list, bool forward)
		{
			int len = list.Count;
			int start;
			int sign;
			int i = 0;
			Func<bool> condition;
			if (forward)
			{
				start = 0;
				sign = +1;
				condition = () => i < len;
			}
			else
			{
				start = len - 1;
				sign = -1;
				condition = () => i >= 0;
			}

			T elementToMove = list[start];
			for (i = start; condition(); i += sign)
			{
				// - get the next element's atIndex
				int nextIndex;
				if (forward)
				{
					nextIndex = (i + 1) % len;
				}
				else
				{
					nextIndex = i - 1;
					if (nextIndex < 0) nextIndex = len - 1;
				}
				// - save next element in a temp variable
				var nextElement = list[nextIndex];
				// - copy the current element over the next
				list[nextIndex] = elementToMove;
				// - update element to move, to the next element
				elementToMove = nextElement;
			}
			return list;
		}
	}
}