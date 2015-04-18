using System;
using System.Collections.Generic;
using Vexe.Runtime.Extensions;

namespace Vexe.Runtime.Types
{
	public class Pool<T> : IDisposable where T : new()
	{
		private List<T> pool;
		private int next;

		public Func<T> CreateItem;
		public Action<T> ResetItem;

		public int Count { get { return pool.Count; } }

		public Pool(int initialCapacity) : this(initialCapacity, () => new T())
		{
		}

		public Pool(int initialCapacity, Func<T> create)
		{
			this.pool = new List<T>(initialCapacity);
			this.CreateItem = create;
		}

		public IDisposable Request(out T item)
		{
			item = Request();
			return this;
		}

		public T Request()
		{
			T result;
			if (next < pool.Count)
			{
				result = pool[next];
				ResetItem.SafeInvoke(result);
			}
			else
			{
				result = CreateItem();
				pool.Add(result);
			}

			next++;
			return result;
		}

		public void Return()
		{
			next--;
		}

		public void Reset()
		{
			next = 0;
		}

		public void Clear()
		{
			pool.Clear();
			Reset();
		}

		public void Dispose()
		{
			Return();
		}
	}
}