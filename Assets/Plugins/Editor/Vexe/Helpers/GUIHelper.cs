using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using BF = System.Reflection.BindingFlags;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Helpers
{
	public static class GUIHelper
	{
		/* <<< Misc >>> */
		#region
		public static bool IsProSkin { get { return EditorGUIUtility.isProSkin; } }

		/// <summary>
		/// Combines two rectangles, returns the result
		/// </summary>
		public static Rect CombineRects(Rect left, Rect right)
		{
			return new Rect(Math.Min(left.x, right.x), Math.Max(left.y, right.y), left.width + right.width, Math.Max(left.height, right.height));
		}

		public static void AddCursorRect(Rect rect, MouseCursor cursor)
		{
			EditorGUIUtility.AddCursorRect(rect, cursor);
		}

		public static Rect GetLastRect()
		{
			return GUILayoutUtility.GetLastRect();
		}
		#endregion

		public static class Folds
		{
			public const string DefaultExpandSymbol = "►";
			public const string DefaultFoldSymbol = "▼";
			public const string AlternateExpandSymbol = ">";
			public const string AlternateFoldSymbol = "˅";
		}

		/* <<< Custom action fields >>> */
		#region
		private static Func<bool> IsLMBMouseDown = EventsHelper.IsLMBMouseDown;
		public static void CustomActionField(Rect fieldRect, Action action, bool showCursorPredicate, bool clickPredicate, MouseCursor cursor)
		{
			if (showCursorPredicate)
				AddCursorRect(fieldRect, cursor);
			if (fieldRect.Contains(Event.current.mousePosition))
			{
				if (clickPredicate)
					action();
			}
		}
		public static void CustomActionField(Action action, bool showCursorPredicate, bool clickPredicate, MouseCursor cursor)
		{
			CustomActionField(GetLastRect(), action, showCursorPredicate, clickPredicate, cursor);
		}
		public static void SelectField(UnityObject obj, MouseCursor cursor = MouseCursor.Link)
		{
			SelectField(GetLastRect(), obj, cursor);
		}
		public static void SelectField(Rect fieldRect, UnityObject obj, MouseCursor cursor = MouseCursor.Link)
		{
			SelectField(fieldRect, obj, IsLMBMouseDown(), cursor);
		}
		public static void SelectField(Rect fieldRect, UnityObject obj, bool clickPredicate, MouseCursor cursor = MouseCursor.Link)
		{
			CustomActionField(fieldRect, () => EditorHelper.SelectObject(obj), obj != null, clickPredicate, cursor);
		}
		public static void SelectField(UnityObject obj, bool clickPredicate, MouseCursor cursor = MouseCursor.Link)
		{
			CustomActionField(() => EditorHelper.SelectObject(obj), obj != null, clickPredicate, cursor);
		}
		public static void PingField(UnityObject obj, MouseCursor cursor = MouseCursor.Zoom)
		{
			PingField(GetLastRect(), obj, cursor);
		}
		public static void PingField(Rect fieldRect, UnityObject obj, MouseCursor cursor = MouseCursor.Zoom)
		{
			PingField(fieldRect, obj, obj != null, IsLMBMouseDown(), cursor);
		}
		public static void PingField(Rect fieldRect, UnityObject obj, bool showPredicate, bool clickPredicate, MouseCursor cursor = MouseCursor.Zoom)
		{
			CustomActionField(fieldRect, () => EditorHelper.PingObject(obj), showPredicate, clickPredicate, cursor);
		}
		public static void PingField(Rect fieldRect, UnityObject obj, bool showPredicate, MouseCursor cursor = MouseCursor.Zoom)
		{
			PingField(fieldRect, obj, showPredicate, IsLMBMouseDown(), cursor);
		}
		public static void PingField(UnityObject obj, bool showPredicate, bool clickPredicate, MouseCursor cursor = MouseCursor.Zoom)
		{
			PingField(GetLastRect(), obj, showPredicate, clickPredicate, cursor);
		}
		public static void PingField(UnityObject obj, bool showPredicate, MouseCursor cursor = MouseCursor.Zoom)
		{
			PingField(obj, showPredicate, IsLMBMouseDown(), cursor);
		}
		#endregion

		/* <<< GUIStyles >>> */
		#region
		private static GUIStyle refreshButtonStyle;
		private static GUIStyle selectedStyle;
		private static GUIStyle selectionButtonStyle;
		public static GUIStyle RefreshButtonStyle
		{
			get
			{
				if (refreshButtonStyle == null)
				{
					refreshButtonStyle = new GUIStyle(EditorStyles.miniButton)
					{
						contentOffset = new Vector2(-1.5f, 0),
						alignment = TextAnchor.MiddleCenter,
						fontSize = 17,
						padding = new RectOffset(0, 0, 0, 0)
					};
				}
				return refreshButtonStyle;
			}
		}
		public static GUIStyle SelectedStyle
		{
			get
			{
				if (selectedStyle == null)
				{
					selectedStyle = new GUIStyle(GUI.skin.textField)
					{
						alignment = TextAnchor.MiddleLeft,
						margin = new RectOffset(0, 0, 0, 0),
						padding = new RectOffset(0, 0, 4, 4),
						normal = new GUIStyleState
						{
							background = RuntimeHelper.GetTexture(61, 128, 223, 255, HideFlags.DontSave),
							textColor = Color.white
						}
					};
				}
				else if (selectedStyle.normal.background == null)
				{
					selectedStyle.normal.background = RuntimeHelper.GetTexture(61, 128, 223, 255, HideFlags.DontSave);
				}
				return selectedStyle;
			}
		}
		public static GUIStyle SelectionButtonStyle
		{
			get
			{
				if (selectionButtonStyle == null)
				{
					selectionButtonStyle = new GUIStyle(EditorStyles.miniButtonRight)
					{
						alignment = TextAnchor.MiddleCenter,
						fontSize = 20,
						padding = new RectOffset(0, 0, 0, 0)
					};
				}
				return selectionButtonStyle;
			}
		}
		public static GUIStyle CreateLabel(int fontSize)
		{
			return CreateLabel(fontSize, Vector2.zero);
		}
		public static GUIStyle CreateLabel(int fontSize, Vector2 contentOffset, TextAnchor alignment = TextAnchor.MiddleLeft, FontStyle fontStyle = FontStyle.Bold)
		{
			return new GUIStyle(GUI.skin.label)
			{
				contentOffset = contentOffset,
				alignment = alignment,
				fontSize = fontSize,
				fontStyle = fontStyle
			};
		}
		public static GUIStyle GetStyle(MiniButtonStyle style)
		{
			switch (style)
			{
				case MiniButtonStyle.Left: return EditorStyles.miniButtonLeft;
				case MiniButtonStyle.Right: return EditorStyles.miniButtonRight;
				case MiniButtonStyle.ModLeft: return ModButtonLeft;
				case MiniButtonStyle.ModRight: return ModButtonRight;
				case MiniButtonStyle.ModMid: return ModButtonMid;
				default: return EditorStyles.miniButtonMid;
			}
		}
		private static GUIStyle GetModButtonStyle(string name, ref GUIStyle style)
		{
			if (style == null)
				style = new GUIStyle(name)
				{
					fontSize = 12,
					contentOffset = new Vector2(-1f, -.8f),
					clipping = TextClipping.Overflow
				};
			return style;
		}
		private static GUIStyle modButtonLeft;
		private static GUIStyle modButtonMid;
		private static GUIStyle modButtonRight;
		private static GUIStyle foldoutStyle;
		public static GUIStyle FoldoutStyle
		{
			get
			{
				if (foldoutStyle == null)
				{
					foldoutStyle = new GUIStyle();
					foldoutStyle.normal = new GUIStyleState
					{
						textColor = EditorStyles.foldout.normal.textColor
					};
				}
				return foldoutStyle;
			}
		}
		public static GUIStyle ModButtonLeft { get { return GetModButtonStyle("miniButtonLeft", ref modButtonLeft); } }
		public static GUIStyle ModButtonMid { get { return GetModButtonStyle("miniButtonMid", ref modButtonMid); } }
		public static GUIStyle ModButtonRight { get { return GetModButtonStyle("miniButtonRight", ref modButtonRight); } }
		public static void DestroyStyleTexture(GUIStyle style)
		{
			UnityObject.DestroyImmediate(style.normal.background);
		}

		/* <<< ColorDuos >>> */
		#region
		private static ColorDuo greenColorDuo;
		private static ColorDuo lightBlueColorDuo;
		private static ColorDuo darkBlueColorDuo;
		private static ColorDuo redColorDuo;
		private static ColorDuo lightGreyColorDuo;
		private static ColorDuo darkGreyColorDuo;
		private static ColorDuo yellowColorDuo;
		private static ColorDuo orangeColorDuo;
		private static ColorDuo pinkColorDuo;
		public static ColorDuo GreenColorDuo { get { return GetColorDuo(ref greenColorDuo, "8AFF8E", "7FE36D"); } }
		public static ColorDuo RedColorDuo { get { return GetColorDuo(ref redColorDuo, "FF9696", "FFB8B8"); } }
		public static ColorDuo LightGreyColorDuo { get { return GetColorDuo(ref lightGreyColorDuo, "C4C4C4", "B8B8B8"); } }
		public static ColorDuo DarkGreyColorDuo { get { return GetColorDuo(ref darkGreyColorDuo, "4A4A4A", "656565"); } }
		public static ColorDuo LightBlueColorDuo { get { return GetColorDuo(ref lightBlueColorDuo, "8FFFFD", "BAFFFE"); } }
		public static ColorDuo DarkBlueColorDuo { get { return GetColorDuo(ref darkBlueColorDuo, "2737B8", "202D91"); } }
		public static ColorDuo YellowColorDuo { get { return GetColorDuo(ref yellowColorDuo, "F7FF69", "FBFFAD"); } }
		public static ColorDuo OrangeColorDuo { get { return GetColorDuo(ref orangeColorDuo, "FFCC99", "FF9933"); } }
		public static ColorDuo PinkColorDuo { get { return GetColorDuo(ref pinkColorDuo, "FFADFA", "FFC9FB"); } }
		private static ColorDuo GetColorDuo(ref ColorDuo cd, string c1, string c2)
		{
			if (cd == null) cd = new ColorDuo(RuntimeHelper.HexToColor(c1), RuntimeHelper.HexToColor(c2));
			return cd;
		}
		#endregion

		/* <<< StyleDuos >>> */
		#region
		private static StyleDuo greenStyleDuo;
		private static StyleDuo redStyleDuo;
		private static StyleDuo lightBlueStyleDuo;
		private static StyleDuo darkBlueStyleDuo;
		private static StyleDuo lightGreyStyleDuo;
		private static StyleDuo darkGreyStyleDuo;
		public static StyleDuo GreenStyleDuo { get { return GetStyleDuo(ref greenStyleDuo, GreenColorDuo); } }
		public static StyleDuo RedStyleDuo { get { return GetStyleDuo(ref redStyleDuo, RedColorDuo); } }
		public static StyleDuo LightBlueStyleDuo { get { return GetStyleDuo(ref lightBlueStyleDuo, LightBlueColorDuo); } }
		public static StyleDuo DarkBlueStyleDuo { get { return GetStyleDuo(ref darkBlueStyleDuo, DarkBlueColorDuo); } }
		public static StyleDuo LightGreyStyleDuo { get { return GetStyleDuo(ref lightGreyStyleDuo, LightGreyColorDuo); } }
		public static StyleDuo DarkGreyStyleDuo { get { return GetStyleDuo(ref darkGreyStyleDuo, DarkGreyColorDuo); } }
		private static StyleDuo GetStyleDuo(ref StyleDuo style, ColorDuo cd)
		{
			// it seems that re-creating the textures if they've been destroyed will still cause some strange leaks
			// so I just re-create the whole style if the textures are destroyed
			if (style == null || style.TexturesHaveBeenDestroyed) style = new StyleDuo(cd);
			return style;
		}
		#endregion
		#endregion

		// Reflection
		#region
		private static MethodCaller<object, object>  _toolbarSearchField_GL;
		public static MethodCaller<object, object> ToolbarSearchField_GL
		{
			get
			{
				return _toolbarSearchField_GL ?? (_toolbarSearchField_GL =
					typeof(EditorGUILayout).DelegateForCall("ToolbarSearchField", Flags.StaticAnyVisibility, typeof(string), typeof(GUILayoutOption[])));
			}
		}

		private static MethodCaller<object, object>  _toolbarSearchField;
		public static MethodCaller<object, object> ToolbarSearchField
		{
			get
			{
				return _toolbarSearchField ?? (_toolbarSearchField =
					typeof(EditorGUI).DelegateForCall("ToolbarSearchField", Flags.StaticAnyVisibility, typeof(Rect), typeof(string[]), typeof(int).MakeByRefType(), typeof(string)));
			}
		}

		private static GUIStyle _selectionRect;
		public static GUIStyle SelectionRect
		{
			get
			{
				return _selectionRect ??
					(_selectionRect = typeof(EditorStyles).GetProperty("selectionRect", Flags.StaticAnyVisibility).GetGetMethod(true).Invoke(null, null) as GUIStyle);
			}
		}

		private static MethodInfo _getHelpIcon;
		public static Texture2D GetHelpIcon(MessageType type)
		{
            if (_getHelpIcon == null)
			    _getHelpIcon = typeof(EditorGUIUtility).GetMethod("GetHelpIcon", BF.Static | BF.NonPublic | BF.Public);
			return _getHelpIcon.Invoke(null, new object[] { type }) as Texture2D;
		}
		#endregion
	}
}

namespace Vexe.Editor
{
	public class ColorDuo
	{
		protected int index;

		private Color[] colors;

		public ColorDuo(Color one, Color two)
		{
			colors = new Color[2];
			colors[0] = one;
			colors[1] = two;
		}

		public void Set(Color one, Color two)
		{
			colors[0] = one;
			colors[1] = two;
			Reset();
		}

		public Color CurrentColor { get { return colors[index]; } }
		public Color FirstColor { get { return colors[0]; } set { colors[0] = value; } }
		public Color SecondColor { get { return colors[1]; } set { colors[1] = value; } }
		public Color NextColor { get { Increment(); return colors[index]; } }
		
		public void Reset()
		{
			index = 0;
		}

		protected void Increment()
		{
			index = (index + 1) % 2;
		}
	}

	public class StyleDuo : ColorDuo
	{
		private GUIStyle[] styles;

		private bool texturesHaveBeenDestroyed;

		public StyleDuo(ColorDuo cd) : this(cd.FirstColor, cd.SecondColor) { }

		public StyleDuo(Color c1, Color c2) : base(c1, c2)
		{
			styles = new GUIStyle[2];
			styles[0] = new GUIStyle(GUIStyle.none)
			{
				normal = new GUIStyleState
				{
					background = RuntimeHelper.GetTexture(c1, HideFlags.HideAndDontSave)
				}
			};
			styles[1] = new GUIStyle(GUIStyle.none)
			{
				normal = new GUIStyleState
				{
					background = RuntimeHelper.GetTexture(c2, HideFlags.HideAndDontSave)
				}
			};
			Reset();
		}

		public bool TexturesHaveBeenDestroyed { get { return texturesHaveBeenDestroyed; } }
		public GUIStyle CurrentStyle { get { return styles[index]; } }
		public GUIStyle FirstStyle { get { return styles[0]; } }
		public GUIStyle SecondStyle { get { return styles[1]; } }
		public GUIStyle NextStyle { get { Increment(); return CurrentStyle; } }

		public void DestroyTextures()
		{
			texturesHaveBeenDestroyed = true;
			UnityObject.DestroyImmediate(FirstStyle.normal.background);
			UnityObject.DestroyImmediate(SecondStyle.normal.background);
		}
	}
}

