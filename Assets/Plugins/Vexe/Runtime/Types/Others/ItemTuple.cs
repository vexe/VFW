using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;

namespace Vexe.Runtime.Types
{
	public struct ItemTuple<T1, T2> 
	{
		public readonly T1 Item1;
		public readonly T2 Item2;

		public ItemTuple(T1 item1, T2 item2)
		{
			this.Item1 = item1;
			this.Item2 = item2;
		} 

		public override string ToString()
		{
			return string.Format("[{0}, {1}]", Item1, Item2);
		}

		public override int GetHashCode()
		{
			return RuntimeHelper.CombineHashCodes(Item1, Item2);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != typeof(ItemTuple<T1, T2>))
				return false;

			return (ItemTuple<T1, T2>)obj == this;
		}

		public static bool operator ==(ItemTuple<T1, T2> left, ItemTuple<T1, T2> right)
		{
			return left.Item1.GenericEquals(right.Item1) && left.Item2.GenericEquals(right.Item2);
		}

		public static bool operator !=(ItemTuple<T1, T2> left, ItemTuple<T1, T2> right)
		{
			return !(left == right);
		}
	}

	public static class ItemTuple
	{
		public static ItemTuple<T1, T2> Create<T1, T2>(T1 item1, T2 second)
		{
			return new ItemTuple<T1, T2>(item1, second);
		}
	}
}