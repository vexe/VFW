using UnityEngine;

namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		public float Float(float value)
		{
			return Float(string.Empty, value);
		}

		public float Float(string label, float value)
		{
			return Float(label, value, null);
		}

		public float Float(string label, string tooltip, float value)
		{
			return Float(label, tooltip, value, null);
		}

		public float Float(string label, float value, Layout option)
		{
			return Float(label, string.Empty, value, option);
		}

		public float Float(string label, string tooltip, float value, Layout option)
		{
			return Float(GetContent(label, tooltip), value, option);
		}

		public abstract float Float(GUIContent content, float value, Layout option);
	}
}