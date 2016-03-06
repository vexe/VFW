//#define PROFILE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
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
		private static string[] Empty = new string[1] { "--empty--" };
		private const string kOwnerTypePrefix = "target";
        private bool _showUpdateButton = true, _changed;
        private TextFilter _filter;

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
                _showUpdateButton = !attribute.HideUpdate;
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
						populateFrom = ReflectionHelper.CachedGetRuntimeTypes()
											           .FirstOrDefault(x => x.Name == typeName);

						if (populateFrom == null)
							throw new InvalidOperationException("Couldn't find type to populate the popup from " + typeName);

						_populateFromType = true;
					}

					fromMember = split[1];
				}

				var all = populateFrom.GetAllMembers(typeof(object));
				var popMember = all.FirstOrDefault(x => attribute.CaseSensitive ? x.Name == fromMember : x.Name.ToLower() == fromMember.ToLower());
				if (popMember == null)
					ErrorHelper.MemberNotFound(populateFrom, fromMember);

				var field = popMember as FieldInfo;
				if (field != null)
					_populateMember = (popMember as FieldInfo).DelegateForGet();
				else
				{
					var prop = popMember as PropertyInfo;
					if (prop != null)
						_populateMember = (popMember as PropertyInfo).DelegateForGet();
					else
					{
						var method = popMember as MethodInfo;
						if (method == null)
							throw new Exception("{0} is not a field, nor a property nor a method!".FormatWith(fromMember));

						_populateMethod = (popMember as MethodInfo).DelegateForCall();
					}
				}
			}
		}

		public override void OnGUI()
		{
			if (memberValue == null)
				memberValue = string.Empty;

			if (_values == null)
            {
				UpdateValues();
                if (attribute.Filter)
                    _filter = new TextFilter(_values, id, false, prefs, SetValue);
            }

            string newValue = null;
            string currentValue = memberValue;

            using (gui.Horizontal())
            {
                if (attribute.TextField)
                {
                    #if PROFILE
                    Profiler.BeginSample("PopupDrawer TextFieldDrop");
                    #endif

                    newValue = gui.TextFieldDropDown(displayText, memberValue, _values);
                    if (currentValue != newValue)
                        _changed = true;

                    #if PROFILE
                    Profiler.EndSample();
                    #endif
                }
                else
                {
                    #if PROFILE
                    Profiler.BeginSample("PopupDrawer TextFieldDrop");
                    #endif

                    if (!_currentIndex.HasValue)
                    {
                        if (attribute.TakeLastPathItem)
                            _currentIndex = _values.IndexOf(x => GetActualValue(x) == currentValue);
                        else
                            _currentIndex = _values.IndexOf(currentValue);
                    }

                    if (_currentIndex == -1)
                    {
                        _currentIndex = 0;
                        if (_values.Length > 0)
                            SetValue(_values[0]);
                    }

                    gui.BeginCheck();
                    int selection = gui.Popup(displayText, _currentIndex.Value, _values);
                    if (gui.HasChanged() && _values.Length > 0)
                    {
                        _currentIndex = selection;
                        _changed = true;
                        newValue = _values[selection];
                    }

                    #if PROFILE
                    Profiler.EndSample();
                    #endif
                }

                if (attribute.Filter)
                    _filter.OnGUI(gui, 45f);

                if (_changed)
                {
                    _changed = false;
                    SetValue(newValue);
                }

                if (_showUpdateButton && gui.MiniButton("U", "Update popup values", MiniButtonStyle.Right))
					UpdateValues();
			}
		}

        private string GetActualValue(string value)
        {
            string result = value;
            if (attribute.TakeLastPathItem && !string.IsNullOrEmpty(value))
            {
                int lastPathIdx = value.LastIndexOf('/') + 1;
                if (lastPathIdx != -1)
                    result = value.Substring(lastPathIdx);
            }
            return result;
        }

        private void SetValue(string value)
        {
            if (!attribute.TextField && attribute.TakeLastPathItem && attribute.Filter)
                _currentIndex = _values.IndexOf(value);

            memberValue = GetActualValue(value);
        }

		public void UpdateValues()
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
			else _values = Empty;
		}

		string[] ProcessPopulation(object obj)
		{
			var arr = obj as string[];
			if (arr != null)
				return arr;

			var list = obj as List<string>;
			if (list != null)
			    return list.ToArray();

            return Empty;
		}
	}
}
