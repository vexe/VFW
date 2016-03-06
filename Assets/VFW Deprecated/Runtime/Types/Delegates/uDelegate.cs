using System;
using System.Collections.Generic;

namespace Vexe.Runtime.Types
{
	[Obsolete("Please use the UnityEvent equivalents instead")]
	public class uDelegate : IBaseDelegate
	{
		/// <summary>
		/// Raw argument values for each handler method to be used in invocation (values set in inspector)
		/// You could modify it at runtime to change the invocation arguments, but you better know what you're doing
		/// </summary>
		public List<object[]> arguments = new List<object[]>();

		public override Type[] ParamTypes
		{
			get { return null; }
		}

		public override Type ReturnType
		{
			get { return typeof(void); }
		}

		public void Invoke()
		{
			for (int i = 0; i < handlers.Count; i++)
			{
				var handler = handlers[i];
				var method = handler.method;
				var target = handler.target;
				if (target == null || method == null)
					continue;

				method.Invoke(target, arguments[i]);
			}
		}
	}
}
