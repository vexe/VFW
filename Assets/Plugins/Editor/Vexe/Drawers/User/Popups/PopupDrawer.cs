using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vexe.Editor.GUIs;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public class PopupDrawer : AttributeDrawer<string, PopupAttribute>
	{
		private string[] _values;
		private int? _currentIndex;
		private MethodCaller<object, object> _populateMethod;
		private MemberGetter<object, object> _populateMember;
		private bool _populateFromTarget, _populateFromType;
		private static string[] NA = new string[1] { "NA" };
		private const string kOwnerTypePrefix = "target";
        private bool _showUpdateButton = true;

		protected override void Initialize()
		{
			string fromMember = attribute.PopulateFrom;
			if (fromMember.IsNullOrEmpty())
			{
				_values = attribute.values;
                _showUpdateButton = false;
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
					if (split[0].ToLower() == kOwnerTypePrefix) // populate from unityTarget
					{ 
						populateFrom = unityTarget.GetType();
						_populateFromTarget = true;
					}
					else // populate from type (member should be static)
					{
						var typeName = split[0];
						populateFrom = AppDomain.CurrentDomain.GetAssemblies()
											.SelectMany(x => x.GetTypes())
											.FirstOrDefault(x => x.Name == typeName);

						if (populateFrom == null)
							throw new InvalidOperationException("Couldn't find type " + typeName);

						_populateFromType = true;
					}

					fromMember = split[1];
				}

				var all = populateFrom.GetAllMembers(typeof(object));
				var member = all.FirstOrDefault(x => attribute.CaseSensitive ? x.Name == fromMember : x.Name.ToLower() == fromMember.ToLower());
				if (member == null)
					throw new vMemberNotFound(populateFrom, fromMember);

				var field = member as FieldInfo;
				if (field != null)
					_populateMember = (member as FieldInfo).DelegateForGet();
				else
				{
					var prop = member as PropertyInfo;
					if (prop != null)
						_populateMember = (member as PropertyInfo).DelegateForGet();
					else
					{
						var method = member as MethodInfo;
						if (method == null)
							throw new Exception("{0} is not a field, nor a property nor a method!".FormatWith(fromMember));

						_populateMethod = (member as MethodInfo).DelegateForCall();
					}
				}
			}
		}

		public override void OnGUI()
		{
			if (memberValue == null)
				memberValue = string.Empty;

			if (_values == null)
				UpdateValues();

            _currentIndex = _values.IndexOf(memberValue);
            if (_currentIndex == -1)
            {
                _currentIndex = 0;
                if (_values.Length > 0)
                    memberValue = _values[0];
            }

			using (gui.Horizontal())
			{
				int x = gui.Popup(displayText, _currentIndex.Value, _values);
				{
					if (_currentIndex != x || (_values.InBounds(x) && memberValue != _values[x]))
					{
						memberValue = _values[x];
						_currentIndex = x;
						gui.RequestResetIfRabbit();
					}
				}

				if (_showUpdateButton && gui.MiniButton("U", "Update popup values", MiniButtonStyle.Right))
					UpdateValues();
			}
		}

		void UpdateValues()
		{
			object target;
			if (_populateFromTarget)
				target = unityTarget;
			else if (_populateFromType)
				target = null;
			else target = rawTarget;

			if (_populateMember != null)
			{
				var pop = _populateMember(target);
				if (pop != null)
					_values = ProcessPopulation(pop);
			}
			else if (_populateMethod != null)
			{
				var pop = _populateMethod(target, null);
				if (pop != null)
					_values = ProcessPopulation(pop);
			}
			else _values = NA;
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