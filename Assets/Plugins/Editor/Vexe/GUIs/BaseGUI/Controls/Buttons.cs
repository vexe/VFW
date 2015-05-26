using System.Collections.Generic;
using UnityEngine;
using Vexe.Editor;
using Vexe.Editor.Helpers;
using Vexe.Runtime.Extensions;

namespace Vexe.Editor
{
	public enum MiniButtonStyle { Left, Mid, Right, ModLeft, ModMid, ModRight }
}


namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		public bool Button(string text)
		{
			return Button(text, (Layout)null);
		}

		public bool Button(string text, GUIStyle style)
		{
			return Button(text, style, null);
		}

		public bool Button(string text, string tooltip, GUIStyle style)
		{
			return Button(text, tooltip, style, null);
		}

		public bool Button(string text, string tooltip, Layout layout)
        {
            return Button(text, tooltip, GUIStyles.Button, layout);
        }

		public bool Button(string text, Layout option)
		{
			return Button(text, GUIStyles.Button, option);
		}

		public bool Button(string text, GUIStyle style, Layout option)
		{
			return Button(text, string.Empty, style, option);
		}

		public bool Button(string text, string tooltip, GUIStyle style, Layout option)
		{
			return Button(GetContent(text, tooltip), style, option);
		}

		public bool Button(GUIContent content, GUIStyle style, Layout option)
		{
			return Button(content, style, option, ControlType.Button);
		}

		public abstract bool Button(GUIContent content, GUIStyle style, Layout option, ControlType buttonType);

		public bool InvisibleButton(Layout option)
		{
			return Button(GUIContent.none, GUIStyle.none, option);
		}

		public bool InvisibleButton()
		{
			return InvisibleButton(null);
		}

		public bool MiniButton(string text)
		{
			return MiniButton(text, string.Empty);
		}

		public bool MiniButton(string text, string tooltip)
		{
			return MiniButton(text, tooltip, kDefaultMiniStyle);
		}

		public bool MiniButton(string text, MiniButtonStyle style)
		{
			return MiniButton(text, style, kDefaultMiniOption);
		}

		public bool MiniButton(string text, Layout option)
		{
			return MiniButton(text, kDefaultMiniStyle, option);
		}

		public bool MiniButton(string text, MiniButtonStyle style, Layout option)
		{
			return MiniButton(text, string.Empty, style, option);
		}

		public bool MiniButton(string text, string tooltip, MiniButtonStyle style)
		{
			return MiniButton(text, tooltip, style, kDefaultMiniOption);
		}

		public bool MiniButton(string text, string tooltip, MiniButtonStyle style, Layout option)
		{
			return MiniButton(GetContent(text, tooltip), style, option);
		}

		public bool MiniButton(GUIContent content, MiniButtonStyle style)
		{
			return MiniButton(content, style, kDefaultMiniOption);
		}

		public bool MiniButton(GUIContent content, MiniButtonStyle style, Layout option)
		{
			return MiniButton(content, GUIHelper.GetStyle(style), option);
		}

		public bool MiniButton(string text, string tooltip, GUIStyle style, Layout option)
		{
			return MiniButton(GetContent(text, tooltip), style, option);
		}

		public bool MiniButton(string text, string tooltip, GUIStyle style)
		{
			return MiniButton(GetContent(text, tooltip), style, kDefaultMiniOption);
		}

		public bool MiniButton(string text, string tooltip, Layout option)
		{
			if (option == null)
				return MiniButton(GetContent(text, tooltip), GUIHelper.GetStyle(kDefaultMiniStyle), Layout.sHeight(kDefaultMiniHeight));
			return MiniButton(GetContent(text, tooltip), GUIHelper.GetStyle(kDefaultMiniStyle), option);
		}

		public bool MiniButton(GUIContent content, GUIStyle style, Layout option)
		{
			return Button(content, style, option, ControlType.MiniButton);
		}

		public bool AddButton(string target)
		{
			return AddButton(target, kDefaultModStyle);
		}

		public bool AddButton(string target, MiniButtonStyle style)
		{
			return MiniButton("+", "Add a new " + target, style);
		}

		public bool ClearButton(string target)
		{
			return ClearButton(target, kDefaultModStyle);
		}

		public bool ClearButton(string target, MiniButtonStyle style)
		{
			return MiniButton("x", "Clear " + target, style);
		}

		public bool RemoveButton(string target)
		{
			return RemoveButton(target, kDefaultModStyle);
		}

		public bool RemoveButton(string target, MiniButtonStyle style)
		{
			return MiniButton("-", "Remove " + target, style);
		}

		public bool MoveUpButton(MiniButtonStyle style)
		{
			return MiniButton("▲", "Move element up", style);
		}

		public bool MoveUpButton()
		{
			return MoveUpButton(kDefaultMiniStyle);
		}

		public bool MoveUpButton<T>(List<T> list, int atIndex, MiniButtonStyle style)
		{
			var move = MoveUpButton(style);
			if (move)
				list.MoveElementUp(atIndex);
			return move;
		}

		public bool MoveUpButton<T>(List<T> list, int atIndex)
		{
			return MoveUpButton(list, atIndex, kDefaultMiniStyle);
		}

		public bool MoveDownButton(MiniButtonStyle style)
		{
			return MiniButton("▼", "Move element down", style);
		}

		public bool MoveDownButton()
		{
			return MoveDownButton(kDefaultMiniStyle);
		}

		public bool MoveDownButton<T>(List<T> list, int atIndex, MiniButtonStyle style)
		{
			var move = MoveDownButton(style);
			if (move)
				list.MoveElementDown(atIndex);
			return move;
		}

		public bool MoveDownButton<T>(List<T> list, int atIndex)
		{
			return MoveDownButton(list, atIndex, kDefaultMiniStyle);
		}

		public bool ToggleButton(bool value, string on, string off, string whatToToggle, MiniButtonStyle style = MiniButtonStyle.Mid)
		{
			var btn = MiniButton(value ? on : off, "Toggle " + whatToToggle, style);
			return btn ? !value : value;
		}

		public bool FoldoutButton(bool value, string whatToFoldout, MiniButtonStyle style = MiniButtonStyle.Mid)
		{
			return ToggleButton(value, GUIHelper.Folds.DefaultFoldSymbol, GUIHelper.Folds.DefaultExpandSymbol, whatToFoldout, style);
		}

		public bool CheckButton(bool value, string whatToToggle)
		{
			return CheckButton(value, whatToToggle, kDefaultMiniStyle);
		}

		public bool CheckButton(bool value, string whatToToggle, MiniButtonStyle style)
		{
			return ToggleButton(value, "✔", string.Empty, whatToToggle, style);
		}

		public bool RefreshButton(Layout option)
		{
			return Button("↶", "Refresh", GUIHelper.RefreshButtonStyle, option);
		}

		public bool RefreshButton()
		{
			return RefreshButton(kDefaultMiniOption);
		}

		public bool SelectionButton(string tooltip, Layout option)
		{
			return Button("◉", tooltip, GUIHelper.SelectionButtonStyle, option);
		}

		public bool SelectionButton(string tooltip)
		{
			return SelectionButton(tooltip, kDefaultMiniOption);
		}

		public bool SelectionButton()
		{
			return SelectionButton(string.Empty);
		}

		public bool InspectButton(GameObject go, MiniButtonStyle style, Layout option)
		{
			var inspect = MiniButton("⏎", "Inspect", style, option);
			if (inspect) EditorHelper.InspectTarget(go);
			return inspect;
		}

		public bool InspectButton(GameObject go, MiniButtonStyle style)
		{
			return InspectButton(go, style, kDefaultMiniOption);
		}

		public bool InspectButton(GameObject go, Layout option)
		{
			return InspectButton(go, kDefaultMiniStyle, option);
		}

		public bool InspectButton(GameObject go)
		{
			return InspectButton(go, kDefaultMiniStyle, kDefaultMiniOption);
		}
	}
}