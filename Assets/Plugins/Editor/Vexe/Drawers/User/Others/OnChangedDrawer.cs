using System;
using System.Linq;
using System.Reflection;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public class OnChangedDrawer : CompositeDrawer<object, OnChangedAttribute>
	{
		private MethodCaller<object, object> onChanged;
		private MemberSetter<object, object> setter;
		private object previous;

		protected override void Initialize()
		{
			string call = attribute.Call;
			string set  = attribute.Set;

			if (!set.IsNullOrEmpty())
			{
				try
				{
					var field = targetType.GetField(set, Flags.InstanceAnyVisibility);
					setter = field.DelegateForSet();
				}
				catch
				{
					try
					{
						var property = targetType.GetProperty(set, Flags.InstanceAnyVisibility);
						setter = property.DelegateForSet();
					}
					catch
					{
						throw new vMemberNotFound(targetType, set);
					}
				}
			}

			if (!call.IsNullOrEmpty())
			{
				try
				{
					var methods = targetType.GetAllMembers(typeof(object)).OfType<MethodInfo>();
					onChanged = (from method in methods
								 where method.Name == call
								 where method.ReturnType == typeof(void)
								 let args = method.GetParameters()
								 where args.Length == 1
								 where args[0].ParameterType.IsAssignableFrom(memberType)
								 select method).FirstOrDefault().DelegateForCall();
				}
				catch
				{
					throw new vMemberNotFound(targetType, call);
				}
			}

			previous = memberValue;
		}

		public override void OnLowerGUI()
		{
			var current = memberValue;
			if (!current.GenericEquals(previous))
			{
				previous = current;
				onChanged.SafeInvoke(rawTarget, current);
				setter.SafeInvoke(ref member.RawTarget, current);
			}
		}
	}
}