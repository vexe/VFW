using UnityEngine;

namespace Vexe.Editor.GUIs
{
	public class VerticalBlock : GUIBlock
	{
		public override void Layout(Rect start)
		{
			int nControls = controls.Count;
			if (nControls == 0)
            { 
                width = height = 0;
				return;
            }

			RectOffset blockMargin = data.style.margin;

			x = start.x;
			y = start.y;

			float nextY = y;
			float totalHeight = 0f, totalWidth = 0f;

			for (int i = 0; i < nControls; i++)
			{
				var control       = controls[i];
				var controlData   = control.data;
				var option        = controlData.option;
				var controlMargin = controlData.style.margin;

				control.width = GetWidth(option, start);
				control.width -= blockMargin.horizontal;

				if (control.width.Value > totalWidth)
					totalWidth = control.width.Value;

				var block = control as GUIBlock;
				if (block != null)
					block.Layout(start);

				control.x = x;
				control.y = nextY;

				var controlHeight = control.height.Value + controlMargin.bottom / 2f;
				nextY		+= controlHeight;
				totalHeight += controlHeight;

				start.y = nextY;
			}

			height = totalHeight + blockMargin.bottom;
			width  = totalWidth + blockMargin.right;
		}

		public override Layout Space(float pxl)
		{
			return new Layout { height = pxl };
		}
	}
}
