using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public class AssignableDrawer : CompositeDrawer<object, AssignableAttribute>
	{
		private int kFoldout, kTarget, kMember, kSource;
		private int targetIdx, memberIdx;
		private GameObject source;
		private Component target;
		private MemberInfo targetMember;

		protected override void Initialize()
		{
			var iden = RuntimeHelper.CombineHashCodes(id, attribute.GetType());
			kSource  = RuntimeHelper.CombineHashCodes(iden, "src");
			kFoldout = RuntimeHelper.CombineHashCodes(iden, "foldout");
			kTarget  = RuntimeHelper.CombineHashCodes(iden, "target");
			kMember  = RuntimeHelper.CombineHashCodes(iden, "member");

			var srcName = prefs.Strings.ValueOrDefault(kSource);
			if (!string.IsNullOrEmpty(srcName))
			{
				source = GameObject.Find(srcName);
				if (source != null)
				{
					var targetTypeName = prefs.Strings.ValueOrDefault(kTarget);
					if (!string.IsNullOrEmpty(targetTypeName))
					{
						target = source.GetComponent(targetTypeName);
						if (target != null)
						{
							var memberName = prefs.Strings.ValueOrDefault(kMember);
							if (!string.IsNullOrEmpty(memberName))
							{ 
								var targetType = target.GetType();
								targetMember = targetType.GetMember(memberName, Flags.InstancePublic).FirstOrDefault();

								targetIdx = source.GetAllComponents().IndexOf(target);
								memberIdx = getMembers(targetType).IndexOf(targetMember);
							}
						}
					}
				}
			}
		}

		public override void OnRightGUI()
		{
			gui.Space(10f);
			foldouts[kFoldout] = gui.Foldout(foldouts[kFoldout]);
		}

		public override void OnLowerGUI()
		{
			if (!foldouts[kFoldout])
				return;

			using (gui.Indent())
			using (gui.Horizontal())
			{
				source = gui.Object<GameObject>(source == null ? "Source" : string.Empty, source);

				if (source != null)
					prefs.Strings[kSource] = source.name;

				if (source != null)
				{
					var targets     = source.GetAllComponents();
					var targetNames = targets.Select(x => x.GetType().Name).ToArray();
					targetIdx       = Mathf.Min(Mathf.Max(0, targetIdx), targets.Length-1);

					targetIdx = gui.Popup(targetIdx, targetNames);
					{
						target = targets[targetIdx];
						if (target != null)
							prefs.Strings[kTarget] = target.GetType().Name;
					}

					if (target != null)
					{
						var type = target.GetType();
						var members = getMembers(type);

						if (members.Count > 0)
						{
							var memberNames = getMemberNames(type);
							memberIdx       = Mathf.Min(Mathf.Max(0, memberIdx), members.Count-1);
							
							memberIdx = gui.Popup(memberIdx, memberNames);
							{
								targetMember = members[memberIdx];
								prefs.Strings[kMember] = targetMember.Name;
							}

							if (targetMember != null)
							{
								object result;
								if (targetMember.MemberType == MemberTypes.Field)
									result = (targetMember as FieldInfo).DelegateForGet().Invoke(target);
								else if (targetMember.MemberType == MemberTypes.Property)
									result = (targetMember as PropertyInfo).DelegateForGet().Invoke(target);
								else result = (targetMember as MethodInfo).DelegateForCall().Invoke(target, null);

								if (!memberValue.GenericEquals(result))
									memberValue = result;
							}
						}
					}
				}
			}
		}

		private Func<Type, string[]> _getMemberNames;
		private Func<Type, string[]> getMemberNames
		{
			get
			{
				return _getMemberNames ?? (_getMemberNames = new Func<Type, string[]>(type =>
					getMembers(type).Select(x => x.Name).ToArray()).Memoize());
			}
		}

		private Func<Type, List<MemberInfo>> _getMembers;
		private Func<Type, List<MemberInfo>> getMembers
		{
			get
			{
				return _getMembers ?? (_getMembers = new Func<Type, List<MemberInfo>>(type =>
				{
					var flags = BindingFlags.Instance | BindingFlags.Public;
					return type.GetFields(flags)
							   .Where(x => x.FieldType.IsA(memberType))
							   .Cast<MemberInfo>()
							   .Concat(
						   type.GetProperties(flags)
							   .Where(x => x.CanRead && x.PropertyType.IsA(memberType))
							   .Cast<MemberInfo>())
							   .Concat(
						   type.GetMethods(flags)
							   .Where(x => x.ReturnType.IsA(memberType) && x.GetParameters().Length == 0 && !x.IsSpecialName)
							   .Cast<MemberInfo>())
							   .ToList();
				}).Memoize());
			}
		}
	}
}