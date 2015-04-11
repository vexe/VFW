using UnityEngine;

namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		public Color Color(Color value)
		{
			return Color(string.Empty, value);
		}

		public Color Color(string label, Color value)
		{
			return Color(label, value, null);
		}

		public Color Color(string label, string tooltip, Color value)
		{
			return Color(label, tooltip, value, null);
		}

		public Color Color(string label, Color value, Layout option)
		{
			return Color(label, string.Empty, value, option);
		}

		public Color Color(string label, string tooltip, Color value, Layout option)
		{
			return Color(GetContent(label, tooltip), value, option);
		}

		public abstract Color Color(GUIContent content, Color value, Layout option);
	}
}