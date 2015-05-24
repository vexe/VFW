using System;
using UnityEngine;

namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		public class ScrollViewBlock : IDisposable
		{
			private readonly BaseGUI gui;

			public ScrollViewBlock(BaseGUI gui)
			{
				this.gui = gui;
			}

			public ScrollViewBlock Begin(ref Vector2 pos)
			{
				return Begin(ref pos, (Layout)null);
			}

			public ScrollViewBlock Begin(ref Vector2 pos, Layout option)
			{
				return Begin(ref pos, GUI.skin.scrollView, option);
			}

			public ScrollViewBlock Begin(ref Vector2 pos, GUIStyle background)
			{
				return Begin(ref pos, background, null);
			}

			public ScrollViewBlock Begin(ref Vector2 pos, GUIStyle background, Layout option)
			{
				return Begin(ref pos, false, false, GUIStyles.HorizontalScrollbar, GUIStyles.VerticalScrollbar, background, option);
			}

			public ScrollViewBlock Begin(ref Vector2 pos, bool alwaysShowHorizontal, bool alwaysShowVertical, Layout option)
			{
				return Begin(ref pos, alwaysShowHorizontal, alwaysShowVertical, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUI.skin.scrollView, option);
			}

			public ScrollViewBlock Begin(ref Vector2 pos, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, Layout option)
			{
				return Begin(ref pos, false, false, horizontalScrollbar, verticalScrollbar, GUIStyles.ScrollView, option);
			}

			public ScrollViewBlock Begin(ref Vector2 pos, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background, Layout option)
			{
				gui.BeginScrollView(ref pos, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background, option);
				return this;
			}

			public void Dispose()
			{
				gui.EndScrollView();
			}
		}

		protected abstract void BeginScrollView(ref Vector2 pos, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background, Layout option);
		protected abstract void EndScrollView();
	}
}