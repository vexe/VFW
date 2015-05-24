using System;
using System.Text.RegularExpressions;
using UnityEngine;
using Vexe.Editor.GUIs;
using Vexe.Editor.Helpers;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public class TextFilter
	{
		private readonly string[] _values;
		private readonly Action<string> _setValue;
        private readonly int _id;
		private string _pattern;
        private bool _toggle;

		public TextFilter(string[] values, int id, Action<string> setValue)
            : this(values, id, true, setValue)
        {
        }

		public TextFilter(string[] values, int id, bool initialToggle, Action<string> setValue)
		{
			this._values   = values;
			this._setValue = setValue;
            this._id = RuntimeHelper.CombineHashCodes(id, "Filter");

            var prefs = BetterPrefs.GetEditorInstance();
            _toggle = prefs.Bools.ValueOrDefault(this._id, initialToggle);
            _pattern = prefs.Strings.ValueOrDefault(this._id, "");
		}

        public bool Field(BaseGUI gui, float width)
        {
            bool changed = false;
            if (_toggle)
            {
                gui.BeginCheck();
                var text = gui.Text(_pattern, Layout.sWidth(width));
                if (gui.HasChanged())
                {
                    changed = true;
                    _pattern = text;

                    var prefs = BetterPrefs.GetEditorInstance();
                    prefs.Strings[_id] = _pattern;
                }
            }
            else gui.Text("", Layout.sWidth(5f));

            var buttonStr = _toggle ? "<" : ">";
            if (gui.Button(buttonStr, GUIStyles.None, Layout.sWidth(13f)))
            {
                _toggle = !_toggle;
                var prefs = BetterPrefs.GetEditorInstance();
                prefs.Bools[_id] = _toggle;
                gui.RequestResetIfRabbit();
            }
            return changed;
        }

        public bool IsMatch(string input)
        {
            try
            {
                return Regex.IsMatch(input, _pattern);
            }
            catch
            {
                return false;
            }
        }

		public void OnGUI(BaseGUI gui)
        {
            OnGUI(gui, 50f);
        }

        public void OnGUI(BaseGUI gui, float width)
		{
            bool changed = Field(gui, width);
			if (changed)
			{
				string match = null;
				for (int i = 0; i < _values.Length; i++)
                {
                    var x = _values[i];
                    if (IsMatch(x))
                    {
                        match = x;
                        break;
                    }
				}

				if (match != null)
					_setValue(match);
			}
		}
	}

	public abstract class FilterDrawer<T, A> : CompositeDrawer<T, A> where A : CompositeAttribute
	{
		private TextFilter filter;

		protected override void Initialize()
		{
			filter = new TextFilter(GetValues(), id, SetValue);
		}

		public override void OnRightGUI()
		{
			filter.OnGUI(gui);
		}

		protected abstract string[] GetValues();
		protected abstract void SetValue(string value);
	}

	public class FilterEnumDrawer : FilterDrawer<Enum, FilterEnumAttribute>
	{
		protected override void SetValue(string value)
		{
			memberValue = Enum.Parse(memberType, value) as Enum;
		}

		protected override string[] GetValues()
		{
			return Enum.GetNames(memberType);
		}
	}

	public class FilterTagsDrawer : FilterDrawer<string, FilterTagsAttribute>
	{
		protected override string[] GetValues()
		{
			return EditorHelper.GetTags();
		}

		protected override void SetValue(string value)
		{
			memberValue = value;
		}
	}
}