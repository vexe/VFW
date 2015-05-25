//#define dbg_level_1

using UnityEngine;

namespace Vexe.Editor.GUIs
{
	using ControlData = BaseGUI.ControlData;

	public class GUIControl
	{
		public float x, y;
		public float? width, height;
		public float vSpacing, hSpacing;
		public ControlData data;

		public float safeHeight { get { return height.HasValue ? height.Value : 0f; } }
		public float safeWidth { get { return width.HasValue ? width.Value : 0f; } }
		public Rect rect
		{
			get
			{
#if dbg_level_1
			if (!width.HasValue || !height.HasValue)
			{
				UnityEngine.Debug.Log("Control {0} has unassigned dimensions. It might have not been layed out yet");
				RabbitGUI.LogCallStack();
				throw new System.Exception();
			}
#endif
				return new Rect(x, y, width.Value, height.Value);
			}
		}

		public GUIControl()
			: this(new ControlData())
		{
		}

		public GUIControl(ControlData data)
		{
			this.data = data;

			vSpacing = BaseGUI.GetVSpacing(data.type);
			hSpacing = BaseGUI.GetHSpacing(data.type);

			width  = null;
			height = null;
		}

		public virtual void ResetDimensions()
		{
			var option = data.option;

			if (option != null)
			{
				if (data.type == BaseGUI.ControlType.PrefixLabel)
					width = UnityEditor.EditorGUIUtility.labelWidth - 5f;
				else if (option.width.HasValue)
					width = option.width.Value;
				height = option.height.HasValue ? option.height.Value : BaseGUI.GetHeight(data.type);
			}
			else
			{
				width  = null;
				height = BaseGUI.GetHeight(data.type);
			}
		}

        public override string ToString()
        {
            return data.type.ToString();
        }
	}
}
