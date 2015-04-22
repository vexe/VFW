using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Vexe.Editor.GUIs;
using Vexe.Editor.Helpers;
using Vexe.Runtime.Extensions;

#pragma warning disable 0414

namespace Vexe.Editor.Windows
{
	public class SelectionWindow : EditorWindow
	{
		public static float IndentWidth;
		public static GUIStyle BackgroundStyle;
		public static Color TabColor;

		static SelectionWindow()
		{
			IndentWidth     = 15f;
			BackgroundStyle = GUIHelper.DarkGreyStyleDuo.SecondStyle;
			TabColor        = GUIHelper.LightBlueColorDuo.SecondColor;
		}

		private bool close;
		private Vector2 scroll;
		private string search;
		private Tab[] tabs;
		private Action onClose;
		private Tab currentTab;
		private BaseGUI gui;
		private static int lastTabIndex;

		private void Initialize(Tab[] tabs, Action onClose)
		{
			// TODO: Use RabbitGUI when we implement BeginScrollView
			gui = new TurtleGUI();

			this.onClose = onClose;
			this.tabs = tabs;

			for (int i = 0; i < tabs.Length; i++)
			{
				var t = tabs[i];
				t.gui = gui;
				t.selectionStyle = GUIHelper.SelectionRect;
			}
			currentTab = lastTabIndex >= tabs.Length ? tabs[0] : tabs[lastTabIndex];
			search = string.Empty;
		}

		public void OnGUI()
		{
			using (gui.Horizontal(EditorStyles.toolbar))
			{
				for (int i = 0; i < tabs.Length; i++)
				{
					var tab = tabs[i];
					using (gui.ColorBlock(tab == currentTab ? TabColor : (Color?)null))
					{
						if (gui.Button(tab.title, EditorStyles.toolbarButton))
						{
							currentTab = tab;
							currentTab.Refresh();
							lastTabIndex = i;
						}
					}
				}
				gui.FlexibleSpace();
			}
			
			gui.Space(3f);
			//GUI.SetNextControlName("SearchBox");
			search = gui.ToolbarSearch(search);
			//if (Event.current != null && Event.current.isKey && Event.current.keyCode == KeyCode.Tab)
			//{
			//	Debug.Log(GUI.GetNameOfFocusedControl());
			//	GUI.FocusControl("SearchBox");
			//	Debug.Log(GUI.GetNameOfFocusedControl());
			//}
			gui.Splitter();
			using (gui.ScrollView.Begin(ref scroll, BackgroundStyle))
				currentTab.OnGUI(search, maxSize.x - minSize.x);
		}

		public void OnFocus()
		{
			if (currentTab != null)
				currentTab.Refresh();
		}

		private void Update()
		{
			if (close) Close(); // Can't close out immediately in OnLostFocus
		}

		public static void Show(string title, Action onClose, params Tab[] tabs)
		{
			GetWindow<SelectionWindow>(true, title).Initialize(tabs, onClose);
		}

		public static void Show(string title, params Tab[] tabs)
		{
			Show(title, null, tabs);
		}

		public static void Show(Action onClose, params Tab[] tabs)
		{
			Show(string.Empty, onClose, tabs);
		}

		public static void Show(params Tab[] tabs)
		{
			Show(string.Empty, null, tabs);
		}

		public void CleanUp()
		{
			GUIHelper.DarkGreyStyleDuo.DestroyTextures();
		}

		private void OnLostFocus()
		{
			CleanUp();
			close = true;
			onClose.SafeInvoke();
		}
	}

	public abstract class Tab
	{
		public string title            { set; get; }
		public BaseGUI gui             { get; set; }
		public GUIStyle selectionStyle { get; set; }

		public virtual void Refresh()
		{
		}

		public abstract void OnGUI(string search, float width);
	}

	public class Tab<T> : Tab
	{
		private readonly Func<T[]> getValues;
		private readonly Action<T> setTarget;
		private readonly Func<T, string> getValueName;
		private readonly Func<T> getCurrent;
		private readonly Func<T, StyleDuo> getStyleDuo;
		private readonly T defaultValue;
		private string previousSearch;
		private T[] values;
		private List<T> filteredValues;

		public Tab(Func<T[]> getValues, Func<T> getCurrent, Action<T> setTarget, Func<T, string> getValueName, Func<T, StyleDuo> getStyleDuo, string title)
		{
			this.getValues    = getValues;
			this.setTarget    = setTarget;
			this.getValueName = getValueName;
			this.getCurrent   = getCurrent;
			this.getStyleDuo  = getStyleDuo ?? (x => GUIHelper.DarkGreyStyleDuo);
			this.title        = title;
			defaultValue      = (T)typeof(T).GetDefaultValue();
		}

		public Tab(Func<T[]> getValues, Func<T> getCurrent, Action<T> setTarget, Func<T, string> getValueName, string title)
			: this(getValues, getCurrent, setTarget, getValueName, null, title)
		{
		}

		public override void OnGUI(string search, float width)
		{
			// Default value
			{
				var isDefault = defaultValue.GenericEquals(getCurrent());
				using (gui.ColorBlock(GUIHelper.RedColorDuo.FirstColor))
				{
					OnValueGUI(defaultValue,
						string.Format("Default ({0})", typeof(T).IsClass ? "Null" : defaultValue.ToString()),
						width, isDefault,
						isDefault ? selectionStyle : getStyleDuo(defaultValue).NextStyle,
						isDefault ? EditorStyles.whiteLargeLabel : EditorStyles.whiteLargeLabel);
				}
			}

			if (values == null) Refresh();
			if (values == null) return;

			if (previousSearch != search)
			{
				previousSearch = search;
				filteredValues = values.Where(x => Regex.IsMatch(getValueName(x), search, RegexOptions.IgnoreCase))
											  .ToList();
			}

			for (int i = 0; i < filteredValues.Count; i++)
			{
				var value      = filteredValues[i];
				var isSelected = value.GenericEquals(getCurrent());
				var nextStyle  = getStyleDuo(value).NextStyle;

				OnValueGUI(value, getValueName(value), width, isSelected, nextStyle,
					isSelected ? EditorStyles.whiteBoldLabel : EditorStyles.whiteLabel);
			}
		}

		private void OnValueGUI(T value, string itemName, float width, bool isSelected, GUIStyle nextStyle, GUIStyle labelStyle)
		{
			using (gui.Horizontal(isSelected ? selectionStyle : nextStyle))
			{
				gui.Space(SelectionWindow.IndentWidth);
				gui.Label(itemName, labelStyle);
				var rect = gui.LastRect;
				{
					rect.width = width;
					if (!isSelected)
					{
						GUIHelper.AddCursorRect(rect, MouseCursor.Link);
						if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
						{
							setTarget(value);
							Refresh();
						}
					}
				}
			}
		}

		public override void Refresh()
		{
			values = getValues();
		}
	}
}