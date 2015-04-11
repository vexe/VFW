using UnityEngine;
using Vexe.Editor.Helpers;
using Vexe.Runtime.Extensions;

namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		public int Popup(int selectedIndex, string[] displayedOptions)
		{
			return Popup(selectedIndex, displayedOptions, null);
		}

		public int Popup(int selectedIndex, string[] displayedOptions, Layout option)
		{
			return Popup(string.Empty, selectedIndex, displayedOptions, option);
		}

		public int Popup(string text, int selectedIndex, string[] displayedOptions)
		{
			return Popup(text, selectedIndex, displayedOptions, null);
		}

		public int Popup(string text, int selectedIndex, string[] displayedOptions, Layout option)
		{
			return Popup(text, selectedIndex, displayedOptions, Styles.Popup, option);
		}

		public abstract int Popup(string text, int selectedIndex, string[] displayedOptions, GUIStyle style, Layout option);

		public string Tag(string tag)
		{
			return Tag(string.Empty, tag);
		}

		public string Tag(string content, string tag)
		{
			return Tag(content, tag, null);
		}

		public string Tag(string content, string tag, Layout layout)
		{
			return Tag(GetContent(content), tag, Styles.Popup, layout);
		}

		public abstract string Tag(GUIContent content, string tag, GUIStyle style, Layout layout);
	}
}
