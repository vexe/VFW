using System;
using System.Reflection;
using Vexe.Runtime.Extensions;

namespace Vexe.Runtime.Types
{
	[Obsolete("Please use the UnityEvent equivalents instead")]
	public class uFunc<TReturn> : uBaseDelegate<Func<TReturn>>
	{
		public override Type[] ParamTypes
		{
			get { return Type.EmptyTypes; }
		}

		public override Type ReturnType
		{
			get { return typeof(TReturn); }
		}

		public TReturn Invoke()
		{
			return Value.SafeInvoke();
		}

		protected override void DirectAdd(Func<TReturn> handler)
		{
			directValue += handler;
		}

		protected override void DirectRemove(Func<TReturn> handler)
		{
			directValue -= handler;
		}

		protected override MethodInfo GetHandlerMethod(Func<TReturn> handler)
		{
			return handler.Method;
		}

		protected override object GetHandlerTarget(Func<TReturn> handler)
		{
			return handler.Target;
		}
	}

	public class uFunc<T0, TReturn> : uBaseDelegate<Func<T0, TReturn>>
	{
		public override Type[] ParamTypes
		{
			get { return new[] { typeof(T0) }; }
		}

		public override Type ReturnType
		{
			get { return typeof(TReturn); }
		}

		public TReturn Invoke(T0 arg0)
		{
			return Value.SafeInvoke(arg0);
		}

		protected override void DirectAdd(Func<T0, TReturn> handler)
		{
			directValue += handler;
		}

		protected override void DirectRemove(Func<T0, TReturn> handler)
		{
			directValue -= handler;
		}

		protected override MethodInfo GetHandlerMethod(Func<T0, TReturn> handler)
		{
			return handler.Method;
		}

		protected override object GetHandlerTarget(Func<T0, TReturn> handler)
		{
			return handler.Target;
		}
	}

	public class uFunc<T0, T1, TReturn> : uBaseDelegate<Func<T0, T1, TReturn>>
	{
		public override Type[] ParamTypes
		{
			get { return new[] { typeof(T0), typeof(T1) }; }
		}

		public override Type ReturnType
		{
			get { return typeof(TReturn); }
		}

		public TReturn Invoke(T0 arg0, T1 arg1)
		{
			return Value.SafeInvoke(arg0, arg1);
		}

		protected override void DirectAdd(Func<T0, T1, TReturn> handler)
		{
			directValue += handler;
		}

		protected override void DirectRemove(Func<T0, T1, TReturn> handler)
		{
			directValue -= handler;
		}

		protected override MethodInfo GetHandlerMethod(Func<T0, T1, TReturn> handler)
		{
			return handler.Method;
		}

		protected override object GetHandlerTarget(Func<T0, T1, TReturn> handler)
		{
			return handler.Target;
		}
	}

	public class uFunc<T0, T1, T2, TReturn> : uBaseDelegate<Func<T0, T1, T2, TReturn>>
	{
		public override Type[] ParamTypes
		{
			get { return new[] { typeof(T0), typeof(T1), typeof(T2) }; }
		}

		public override Type ReturnType
		{
			get { return typeof(TReturn); }
		}

		public TReturn Invoke(T0 arg0, T1 arg1, T2 arg2)
		{
			return Value.SafeInvoke(arg0, arg1, arg2);
		}

		protected override void DirectAdd(Func<T0, T1, T2, TReturn> handler)
		{
			directValue += handler;
		}

		protected override void DirectRemove(Func<T0, T1, T2, TReturn> handler)
		{
			directValue -= handler;
		}

		protected override MethodInfo GetHandlerMethod(Func<T0, T1, T2, TReturn> handler)
		{
			return handler.Method;
		}

		protected override object GetHandlerTarget(Func<T0, T1, T2, TReturn> handler)
		{
			return handler.Target;
		}
	}

	public class uFunc<T0, T1, T2, T3, TReturn> : uBaseDelegate<Func<T0, T1, T2, T3, TReturn>>
	{
		public override Type[] ParamTypes
		{
			get { return new[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3) }; }
		}

		public override Type ReturnType
		{
			get { return typeof(TReturn); }
		}

		public TReturn Invoke(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
		{
			return Value.SafeInvoke(arg0, arg1, arg2, arg3);
		}

		protected override void DirectAdd(Func<T0, T1, T2, T3, TReturn> handler)
		{
			directValue += handler;
		}

		protected override void DirectRemove(Func<T0, T1, T2, T3, TReturn> handler)
		{
			directValue -= handler;
		}

		protected override MethodInfo GetHandlerMethod(Func<T0, T1, T2, T3, TReturn> handler)
		{
			return handler.Method;
		}

		protected override object GetHandlerTarget(Func<T0, T1, T2, T3, TReturn> handler)
		{
			return handler.Target;
		}
	}
}