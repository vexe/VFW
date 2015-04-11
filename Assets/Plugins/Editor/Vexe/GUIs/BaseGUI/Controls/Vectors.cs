using UnityEngine;


namespace Vexe.Editor.GUIs
{
	public abstract partial class BaseGUI
	{
		public Vector3 Vector3(Vector3 value)
		{
			return Vector3(string.Empty, value);
		}

		public Vector3 Vector3(string label, Vector3 value)
		{
			return Vector3(label, value, null);
		}

		public Vector3 Vector3(string label, string tooltip, Vector3 value)
		{
			return Vector3(label, tooltip, value, null);
		}

		public Vector3 Vector3(string label, Vector3 value, Layout option)
		{
			return Vector3(label, string.Empty, value, option);
		}

		public Vector3 Vector3(string label, string tooltip, Vector3 value, Layout option)
		{
			return Vector3(GetContent(label, tooltip), value, option);
		}

		public Vector3 Vector3(GUIContent content, Vector3 value, Layout option)
		{
			using (Horizontal())
			{
				Prefix(content.text);
				using (LabelWidth(13f))
				{
					value.x = Float("X", value.x);
					value.y = Float("Y", value.y);
					value.z = Float("Z", value.z);
				}
			}

			return value;
		}

		public Vector2 Vector2(Vector2 value)
		{
			return Vector2(string.Empty, value);
		}

		public Vector2 Vector2(string label, Vector2 value)
		{
			return Vector2(label, value, null);
		}

		public Vector2 Vector2Field(string label, string tooltip, Vector2 value)
		{
			return Vector2(label, tooltip, value, null);
		}

		public Vector2 Vector2(string label, Vector2 value, Layout option)
		{
			return Vector2(label, string.Empty, value, option);
		}

		public Vector2 Vector2(string label, string tooltip, Vector2 value, Layout option)
		{
			return Vector2(GetContent(label, tooltip), value, option);
		}

		public Vector2 Vector2(GUIContent content, Vector2 value, Layout option)
		{
			using (Horizontal())
			{
				Prefix(content.text);
				using (LabelWidth(13f))
				{
					value.x = Float("X", value.x);
					value.y = Float("Y", value.y);
				}
			}
			return value;
		}
	}
}
