using UnityEngine;
using Vexe.Runtime.Extensions;

namespace Vexe.Runtime.Types
{
	public struct Tuple<T1, T2>
	{
		[SerializeField] readonly private T1 _item1;
		[SerializeField] readonly private T2 _item2;

		public T1 Item1 { get { return _item1; } }
		public T2 Item2 { get { return _item2; } }

		public Tuple(T1 item1, T2 item2)
		{
			_item1 = item1;
			_item2 = item2;
		}

		public override string ToString()
		{
			return string.Format("[{0}, {1}]", Item1, Item2);
		}

		public override int GetHashCode()
		{
			return Item1.GetHashCode() ^ Item2.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != typeof(Tuple<T1, T2>))
			{
				return false;
			}
			var tuple = (Tuple<T1, T2>)obj;
			return this == tuple;
		}

		public static bool operator ==(Tuple<T1, T2> left, Tuple<T1, T2> right)
		{
			return left.Item1.GenericEqual(right.Item1) && left.Item2.GenericEqual(right.Item2);
		}

		public static bool operator !=(Tuple<T1, T2> left, Tuple<T1, T2> right)
		{
			return !(left == right);
		}
	}

	public static class Tuple
	{
		public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 second)
		{
			return new Tuple<T1, T2>(item1, second);
		}
	}
}