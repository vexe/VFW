using UnityEngine;

namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		public int Int(int value)
		{
			return Int(string.Empty, value);
		}

		public int Int(string label, int value)
		{
			return Int(label, value, null);
		}

		public int Int(string label, string tooltip, int value)
		{
			return Int(label, tooltip, value, null);
		}

		public int Int(string label, int value, Layout option)
		{
			return Int(label, string.Empty, value, option);
		}

		public int Int(string label, string tooltip, int value, Layout option)
		{
			return Int(GetContent(label, tooltip), value, option);
		}

		public abstract int Int(GUIContent content, int value, Layout option);
	}
}