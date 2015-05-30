using System;
using System.Linq;
using System.Reflection;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public class OnChangedDrawer : CompositeDrawer<object, OnChangedAttribute>
	{
		private MethodCaller<object, object> _onChanged;
		private MemberSetter<object, object> _setter;
		private object _previousValue;
        private int _previousCollectionCount;

		protected override void Initialize()
		{
			string call = attribute.Call;
			string set  = attribute.Set;

			if (!set.IsNullOrEmpty())
			{
				try
				{
					var field = targetType.GetField(set, Flags.InstanceAnyVisibility);
					_setter = field.DelegateForSet();
				}
				catch
				{
					try
					{
						var property = targetType.GetProperty(set, Flags.InstanceAnyVisibility);
						_setter = property.DelegateForSet();
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
					_onChanged = (from method in methods
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

			_previousValue = memberValue;

            if (member.CollectionCount != -1)
                _previousCollectionCount = member.CollectionCount;
		}

		public override void OnLowerGUI()
		{
			var current = memberValue;

            bool changed;
            if (member.CollectionCount != -1 && member.CollectionCount != _previousCollectionCount)
            {
                _previousCollectionCount = member.CollectionCount;
                changed = true;
            }
            else
                changed = !current.GenericEquals(_previousValue);

            if (changed)
			{
				_previousValue = current;
				_onChanged.SafeInvoke(rawTarget, current);
				_setter.SafeInvoke(ref member.RawTarget, current);
			}
		}
	}
}