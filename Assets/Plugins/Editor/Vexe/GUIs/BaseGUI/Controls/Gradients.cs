using UnityEngine;

namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		public Gradient GradientField(Gradient value)
		{
			return GradientField(string.Empty, value);
		}

		public Gradient GradientField(string label, Gradient value)
		{
			return GradientField(label, value, Layout.None);
		}

		public Gradient GradientField(string label, string tooltip, Gradient value)
		{
			return GradientField(label, tooltip, value, Layout.None);
		}

		public Gradient GradientField(string label, Gradient value, Layout option)
		{
			return GradientField(label, string.Empty, value, option);
		}

		public Gradient GradientField(string label, string tooltip, Gradient value, Layout option)
		{
			return GradientField(GetContent(label, tooltip), value, option);
		}

		public abstract Gradient GradientField(GUIContent content, Gradient value, Layout option);
	}
}
