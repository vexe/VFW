using System;
using System.Collections.Generic;
using System.Reflection;
using Vexe.Runtime.Extensions;
using UnityObject = UnityEngine.Object;

namespace Vexe.Runtime.Types
{
	public abstract class IBaseDelegate
	{
		public List<Handler> handlers = new List<Handler>();

		public abstract Type[] ParamTypes { get; }
		public abstract Type ReturnType { get; }

		public class Handler
		{
			public object target;
			public MethodInfo method;
		}
	}

	public abstract class uBaseDelegate<T> : IBaseDelegate where T : class
	{
		protected T directValue;

		protected T Value
		{
			set { directValue = value; }
			get
			{
				if (directValue == null)
					Rebuild();
				return directValue;
			}
		}

		public void Add(T handler)
		{
			handlers.Add(new Handler
			{
				target = GetHandlerTarget(handler),
				method = GetHandlerMethod(handler)
			});
			DirectAdd(handler);
		}

		public void Remove(T handler)
		{
			int index = handlers.IndexOf(t => t.target == GetHandlerTarget(handler));
			if (index == -1) return;
			handlers.RemoveAt(index);
			DirectRemove(handler);
		}

		public bool Contains(T handler)
		{
			int idx = handlers.FindIndex(t => t.target == GetHandlerTarget(handler) &&
											  t.method == GetHandlerMethod(handler));
			return idx != -1;
		}

		public void Clear()
		{
			directValue = null;
			handlers.Clear();
		}

		public void Rebuild()
		{
			directValue = null;
			for (int i = 0; i < handlers.Count; i++)
			{
				var handler = handlers[i];
				var del     = Delegate.CreateDelegate(typeof(T), handler.target, handler.method) as T;
				DirectAdd(del);
			}
		}

		protected abstract MethodInfo GetHandlerMethod(T handler);
		protected abstract object GetHandlerTarget(T handler);
		protected abstract void DirectAdd(T handler);
		protected abstract void DirectRemove(T handler);
	}
}