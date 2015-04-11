using UnityEngine;

namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		public Rect Rect(Rect value)
		{
			return Rect(string.Empty, value);
		}

		public Rect Rect(string label, Rect value)
		{
			return Rect(label, value, kMultifieldOption);
		}

		public Rect Rect(string label, string tooltip, Rect value)
		{
			return Rect(label, tooltip, value, kMultifieldOption);
		}

		public Rect Rect(string label, Rect value, Layout option)
		{
			return Rect(label, string.Empty, value, option);
		}

		public Rect Rect(string label, string tooltip, Rect value, Layout option)
		{
			return Rect(new GUIContent(label, tooltip), value, option);
		}

		public abstract Rect Rect(GUIContent content, Rect value, Layout option);
	}
}
