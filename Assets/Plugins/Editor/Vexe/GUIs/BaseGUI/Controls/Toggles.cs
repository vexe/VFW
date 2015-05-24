using UnityEngine;

namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		public bool Toggle(bool value)
		{
			return Toggle(string.Empty, value);
		}

		public bool Toggle(string label, bool value)
		{
			return Toggle(label, value, (Layout)null);
		}

		public bool Toggle(string label, bool value, Layout option)
		{
			return Toggle(label, value, string.Empty, option);
		}

		public bool Toggle(string label, bool value, string tooltip)
		{
			return Toggle(label, value, tooltip, null);
		}

		public bool Toggle(string label, bool value, string tooltip, Layout option)
		{
			return Toggle(GetContent(label, tooltip), value, option);
		}

		public bool Toggle(GUIContent content, bool value, Layout option)
		{
			return Toggle(content, value, GUIStyles.Toggle, option);
		}

		public abstract bool Toggle(GUIContent content, bool value, GUIStyle style, Layout option);


		public bool ToggleLeft(string label, bool value)
		{
			return ToggleLeft(label, value, null);
		}

		public bool ToggleLeft(string label, bool value, Layout option)
		{
			return ToggleLeft(label, value, GUIStyles.BoldLabel, option);
		}

		public bool ToggleLeft(string label, bool value, GUIStyle labelStyle, Layout option)
		{
			return ToggleLeft(GetContent(label), value, labelStyle, option);
		}

		public abstract bool ToggleLeft(GUIContent content, bool value, GUIStyle labelStyle, Layout option);
	}
}