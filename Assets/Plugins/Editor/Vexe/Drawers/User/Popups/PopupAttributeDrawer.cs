using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vexe.Editor.GUIs;
using Vexe.Runtime.Exceptions;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public class PopupAttributeDrawer : AttributeDrawer<string, PopupAttribute>
	{
		private string[] values;
		private int? currentIndex;
		private MethodCaller<object, object> populateMethod;
		private MemberGetter<object, object> populateMember;
		private bool populateFromTarget, populateFromType;
		private static string[] NA = new string[1] { "NA" };
		private const string OwnerTypePrefix = "target";

		protected override void OnSingleInitialization()
		{
			string fromMember = attribute.PopulateFrom;
			if (fromMember.IsNullOrEmpty())
			{
				values = attribute.values;
			}
			else
			{
				Type populateFrom;
				var split = fromMember.Split('.');
				if (split.Length == 1)
				{
					populateFrom = targetType;
				}
				else
				{
					if (split[0].ToLower() == OwnerTypePrefix) // populate from unityTarget
					{ 
						populateFrom = unityTarget.GetType();
						populateFromTarget = true;
					}
					else // populate from type (member should be static)
					{
						var typeName = split[0];
						populateFrom = AppDomain.CurrentDomain.GetAssemblies()
											.SelectMany(x => x.GetTypes())
											.FirstOrDefault(x => x.Name == typeName);

						if (populateFrom == null)
							throw new InvalidOperationException("Couldn't find type " + typeName);

						populateFromType = true;
					}

					fromMember = split[1];
				}

				var all = populateFrom.GetAllMembers(typeof(object));
				var member = all.FirstOrDefault(x => attribute.CaseSensitive ? x.Name == fromMember : x.Name.ToLower() == fromMember.ToLower());
				if (member == null)
					throw new MemberNotFoundException(fromMember);

				var field = member as FieldInfo;
				if (field != null)
					populateMember = (member as FieldInfo).DelegateForGet();
				else
				{
					var prop = member as PropertyInfo;
					if (prop != null)
						populateMember = (member as PropertyInfo).DelegateForGet();
					else
					{
						var method = member as MethodInfo;
						if (method == null)
							throw new WTFException("{0} is not a field, nor a property nor a method!".FormatWith(fromMember));

						populateMethod = (member as MethodInfo).DelegateForCall();
					}
				}
			}
		}

		public override void OnGUI()
		{
			if (memberValue == null)
				memberValue = string.Empty;

			if (values == null)
				UpdateValues();

			//if (!currentIndex.HasValue)
			{
				currentIndex = values.IndexOf(memberValue);
				if (currentIndex == -1)
				{
					currentIndex = 0;
					if (values.Length > 0)
						memberValue = values[0];
				}
			}

			using (gui.Horizontal())
			{
				int x = gui.Popup(niceName, currentIndex.Value, values);
				{
					if (currentIndex != x || (values.InBounds(x) && memberValue != values[x]))
					{
						memberValue = values[x];
						currentIndex = x;
						gui.RequestResetIfRabbit();
					}
				}

				if (gui.MiniButton("U", "Update popup values", MiniButtonStyle.Right))
					UpdateValues();
			}
		}

		void UpdateValues()
		{
			object target;
			if (populateFromTarget)
				target = unityTarget;
			else if (populateFromType)
				target = null;
			else target = rawTarget;

			if (populateMember != null)
			{
				var pop = populateMember(target);
				if (pop != null)
					values = ProcessPopulation(pop);
			}
			else if (populateMethod != null)
			{
				var pop = populateMethod(target, null);
				if (pop != null)
					values = ProcessPopulation(pop);
			}
			else values = NA;
		}

		string[] ProcessPopulation(object obj)
		{
			var arr = obj as string[];
			if (arr != null)
				return arr;

			var list = obj as List<string>;
			if (list == null)
				return NA;

			return list.ToArray();
		}

	}
}