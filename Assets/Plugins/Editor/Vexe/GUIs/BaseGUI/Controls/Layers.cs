using UnityEngine;

namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		public int Layer(int layer)
		{
			return Layer(string.Empty, layer);
		}

		public int Layer(string label, int layer)
		{
			return Layer(label, layer, null);
		}

		public LayerMask Layer(string content, LayerMask layer)
		{
			return Layer(content, (int)layer);
		}

		public int Layer(string label, int layer, Layout layout)
		{
			return Layer(label, layer, GUIStyles.Popup, layout);
		}

		public int Layer(string label, int layer, GUIStyle style, Layout layout)
		{
			return Layer(GetContent(label), layer, style, layout);
		}

		public abstract int Layer(GUIContent label, int layer, GUIStyle style, Layout layout);
	}
}