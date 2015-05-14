using UnityEngine;

namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		public double Double(double value)
		{
			return Double(string.Empty, value);
		}

		public double Double(string label, double value)
		{
			return Double(label, value, null);
		}

		public double Double(string label, string tooltip, double value)
		{
			return Double(label, tooltip, value, null);
		}

		public double Double(string label, double value, Layout option)
		{
			return Double(label, string.Empty, value, option);
		}

		public double Double(string label, string tooltip, double value, Layout option)
		{
			return Double(GetContent(label, tooltip), value, option);
		}

		public abstract double Double(GUIContent content, double value, Layout option);
	}
}