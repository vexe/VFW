using System;
using System.Collections.Generic;
using System.Reflection;
using UnityObject = UnityEngine.Object;

namespace Vexe.Runtime.Types
{
	public abstract class BaseDelegate
	{
		public List<Handler> handlers = new List<Handler>();

		public abstract Type[] ParamTypes { get; }
		public abstract Type ReturnType   { get; }

		public class Handler
		{
			public UnityObject target;
			public MethodInfo method;
		}
	}
}