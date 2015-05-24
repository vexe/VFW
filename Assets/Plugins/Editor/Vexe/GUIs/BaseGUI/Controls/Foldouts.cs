using UnityEngine;
using Vexe.Editor.Helpers;


namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		public bool Foldout(bool value)
		{
			return Foldout(string.Empty, value);
		}

		public bool Foldout(bool value, Layout option)
		{
			return Foldout(string.Empty, value, option);
		}

		public bool Foldout(string label, bool value)
		{
			return Foldout(label, value, GUIStyles.Foldout);
		}

		public bool Foldout(string label, bool value, GUIStyle style)
		{
			return Foldout(label, value, string.Empty, style, kFoldoutOption);
		}

		public bool Foldout(string label, bool value, Layout option)
		{
			return Foldout(label, value, string.Empty, GUIStyles.Foldout, option);
		}

		public bool Foldout(string label, bool value, GUIStyle style, Layout option)
		{
			return Foldout(label, value, string.Empty, style, option);
		}

		public bool Foldout(string label, bool value, string tooltip, GUIStyle style, Layout option)
		{
			return Foldout(GetContent(label, tooltip), value, style, option);
		}

		public abstract bool Foldout(GUIContent content, bool value, GUIStyle style, Layout option);

		public bool CustomFoldout(string label, bool value, string expandSymbol, string foldSymbol, GUIStyle style, Layout option)
		{
			Label((value ? foldSymbol : expandSymbol) + label, GUIHelper.FoldoutStyle, option);
			if (GUI.Button(LastRect, GUIContent.none, GUIStyle.none))
				value = !value;
			return value;
		}

		public bool CustomFoldout(string expandSymbol, string foldSymbol, bool value)
		{
			return CustomFoldout(string.Empty, value, expandSymbol, foldSymbol, null, kFoldoutOption);
		}

		public bool CustomFoldout(string label, bool value, GUIStyle style, Layout option)
		{
			return CustomFoldout(label, value, GUIHelper.Folds.DefaultExpandSymbol, GUIHelper.Folds.DefaultFoldSymbol, style, option);
		}

		public bool CustomFoldout(string label, bool value, Layout option)
		{
			return CustomFoldout(label, value, GUIStyle.none, option);
		}

		public bool CustomFoldout(bool value, Layout option)
		{
			return CustomFoldout(string.Empty, value, option);
		}

		public bool CustomFoldout(bool value)
		{
			return CustomFoldout(string.Empty, value, kFoldoutOption);
		}


		public abstract bool InspectorTitlebar(bool foldout, Object target);
	}
}