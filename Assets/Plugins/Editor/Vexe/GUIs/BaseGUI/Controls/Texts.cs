using UnityEngine;

namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		public string Text(string value)
		{
			return Text(string.Empty, value);
		}

		public string Text(string label, string value)
		{
			return Text(label, value, Layout.Auto);
		}

		public string Text(string value, Layout option)
        {
            return Text(GUIContent.none, value, option);
        }

		public string Text(string label, string value, Layout option)
		{
			return Text(label, value, string.Empty, option);
		}

		public string Text(string label, string value, string tooltip)
		{
			return Text(label, value, tooltip, Layout.Auto);
		}

		public string Text(string label, string value, string tooltip, Layout option)
		{
			return Text(GetContent(label, tooltip), value, option);
		}

		public string Text(GUIContent content, string value, Layout option)
		{
			return Text(content, value, GUIStyles.TextField, option);
		}

		public abstract string Text(GUIContent content, string value, GUIStyle style, Layout option);

		public string ToolbarSearch(string value)
		{
			return ToolbarSearch(value, null);
		}

		public abstract string ToolbarSearch(string value, Layout option);

		public string TextArea(string value)
		{
			return TextArea(value, null);
		}

		public abstract string TextArea(string value, Layout option);

		public string ScrollableTextArea(string value, ref Vector2 scrollPos, Layout option)
		{
			return ScrollableTextArea(value, ref scrollPos, GUIStyles.TextArea, option);
		}

		public abstract string ScrollableTextArea(string value, ref Vector2 scrollPos, GUIStyle style, Layout option);
	}
}