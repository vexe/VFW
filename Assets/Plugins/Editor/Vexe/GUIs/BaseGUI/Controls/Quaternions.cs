using UnityEngine;

namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		public Quaternion Quaternion(Quaternion value)
		{
			return Quaternion(string.Empty, value);
		}

		public Quaternion Quaternion(string label, Quaternion value)
		{
			return Quaternion(label, value, null);
		}

		public Quaternion Quaternion(string label, string tooltip, Quaternion value)
		{
			return Quaternion(label, tooltip, value, null);
		}

		public Quaternion Quaternion(string label, Quaternion value, Layout option)
		{
			return Quaternion(label, string.Empty, value, option);
		}

		public Quaternion Quaternion(string label, string tooltip, Quaternion value, Layout option)
		{
			return Quaternion(GetContent(label, tooltip), value, option);
		}

		public Quaternion Quaternion(GUIContent content, Quaternion value, Layout option)
		{
			return UnityEngine.Quaternion.Euler(Vector3(content, value.eulerAngles, option));
		}
	}
}