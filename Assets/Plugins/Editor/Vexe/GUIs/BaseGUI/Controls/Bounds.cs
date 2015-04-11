using UnityEngine;


namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		public Bounds BoundsField(Bounds value)
		{
			return BoundsField(string.Empty, value);
		}

		public Bounds BoundsField(string label, Bounds value)
		{
			return BoundsField(label, value, kMultifieldOption);
		}

		public Bounds BoundsField(string label, string tooltip, Bounds value)
		{
			return BoundsField(label, tooltip, value, kMultifieldOption);
		}

		public Bounds BoundsField(string label, Bounds value, Layout option)
		{
			return BoundsField(label, string.Empty, value, option);
		}

		public Bounds BoundsField(string label, string tooltip, Bounds value, Layout option)
		{
			return BoundsField(GetContent(label, tooltip), value, option);
		}

		public abstract Bounds BoundsField(GUIContent content, Bounds value, Layout option);
	}
}