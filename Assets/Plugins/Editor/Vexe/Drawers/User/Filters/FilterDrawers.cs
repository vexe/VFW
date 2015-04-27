using System;
using System.Text.RegularExpressions;
using UnityEngine;
using Vexe.Editor.GUIs;
using Vexe.Editor.Helpers;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public class FieldFilter
	{
		private readonly string[] values;
		private readonly Action<string> setValue;
		private string search;

		public FieldFilter(string[] values, Action<string> setValue)
		{
			this.values   = values;
			this.setValue = setValue;
		}

		public void OnGUI(BaseGUI gui)
		{
			var newSearch = gui.Text(GUIContent.none, search, Layout.sWidth(50f));
			if (search != newSearch)
			{
				string match = null;
				for (int i = 0; i < values.Length; i++)
				{
					var x = values[i];
					if (Regex.IsMatch(x, newSearch))
					{
						match = x;
						break;
					}
				}

				search = newSearch;

				if (match != null)
				{
					setValue(match);
				}
			}
		}
	}

	public abstract class FilterDrawer<T, A> : CompositeDrawer<T, A> where A : CompositeAttribute
	{
		private FieldFilter filter;

		protected override void Initialize()
		{
			filter = new FieldFilter(GetValues(), SetValue);
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