using UnityEditor;
using UnityEngine;


namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		public void NumericLabel(int n, GUIStyle style)
		{
			Label(n + "- ", style, new Layout { width = kNumericLabelWidth });
		}

		public void NumericLabel(int n)
		{
			NumericLabel(n, EditorStyles.miniLabel);
		}

		public void NumericTextFieldLabel(int n, string label)
		{
			using (Horizontal())
			{
				NumericLabel(n);
				TextLabel(label);
			}
		}

		public void Prefix()
		{
			Prefix(string.Empty);
		}

		public abstract void Prefix(string label);

		public void TextLabel(string label)
		{
			Label(label, GUI.skin.textField);
		}

		public void InactiveText(string label, string field)
		{
			Prefix(label);
			Label(field, GUI.skin.textField);
		}

		public void BoldLabel(string text)
		{
			BoldLabel(text, null);
		}

		public void BoldLabel(string text, Layout option)
		{
			Label(text, GUIStyles.BoldLabel, option);
		}

		public void Label(string text)
		{
			Label(text, (Layout)null);
		}

		public void Label(string text, Layout option)
		{
			Label(text, GUIStyles.Label, option);
		}

		public void Label(string text, GUIStyle style)
		{
			Label(text, style, null);
		}

		public void Label(string text, GUIStyle style, Layout option)
		{
			Label(GetContent(text), style, option);
		}

		public void Label(GUIContent content, GUIStyle style)
        {
            Label(content, style, null);
        }

		public abstract void Label(GUIContent content, GUIStyle style, Layout option);
	}
}