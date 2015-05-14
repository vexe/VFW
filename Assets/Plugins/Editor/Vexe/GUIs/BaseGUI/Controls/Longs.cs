using UnityEngine;

namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		public long Long(long value)
		{
			return Long(string.Empty, value);
		}

		public long Long(string label, long value)
		{
			return Long(label, value, null);
		}

		public long Long(string label, string tooltip, long value)
		{
			return Long(label, tooltip, value, null);
		}

		public long Long(string label, long value, Layout option)
		{
			return Long(label, string.Empty, value, option);
		}

		public long Long(string label, string tooltip, long value, Layout option)
		{
			return Long(GetContent(label, tooltip), value, option);
		}

		public abstract long Long(GUIContent content, long value, Layout option);
	}
}