//#define dbg

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vexe.Runtime.Extensions;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System;

namespace Vexe.Editor.GUIs
{
	using ControlType = BaseGUI.ControlType;

	public class HorizontalBlock : GUIBlock
	{
		static List<int> nonDefIndicies = new List<int>();

		public override void Layout(Rect start)
		{
#if dbg
		var watch = Stopwatch.StartNew();
#endif

			int totalControls = controls.Count;
			if (totalControls == 0)
            { 
                height = width = 0;
				return;
            }

			var margin = data.style.margin;

			nonDefIndicies.Clear();

			float totalDefinedWidth = 0f, totalSpace = 0f;
			int nDefWidth = 0, nFlexibles = 0;
			for (int i = 0; i < totalControls; i++)
			{
				var control = controls[i];

				totalSpace += control.hSpacing;

				if (control.data.type == ControlType.FlexibleSpace)
				{
					nFlexibles++;
				}
				else if (control.width.HasValue)
				{
					totalDefinedWidth += control.width.Value;
					nDefWidth++;
				}
				else
				{
					nonDefIndicies.Add(i);
				}
			}

			totalSpace -= controls[totalControls - 1].hSpacing;

			float flexOrUnified = 0;
			if (nFlexibles > 0)
			{
				float totalWidthTaken = 0;

				for (int i = 0; i < nonDefIndicies.Count; i++)
				{
					var c = controls[nonDefIndicies[i]];
					float w = c.data.style.CalcSize(c.data.content).x;
					c.width = w;
					totalWidthTaken += w;
				}

				float leftoverSpace = GetWidth(null, start) - totalSpace - margin.horizontal - totalWidthTaken - totalDefinedWidth;
				flexOrUnified = leftoverSpace / nFlexibles;
			}
			else
			{
				flexOrUnified = (GetWidth(null, start) - totalDefinedWidth - totalSpace - margin.horizontal) /
											(totalControls - nDefWidth);
			}

			x = start.x + margin.left;
			y = start.y + margin.top;

			float nextX = x;

			for (int i = 0; i < totalControls; i++)
			{
				var control = controls[i];

				control.x = nextX;
				control.y = y;

				if (!control.width.HasValue)
					control.width = flexOrUnified;

				var block = control as GUIBlock;
				if (block != null)
				{
					block.Layout(start);
				}

				float controlHeight = control.height.Value;
				if (controlHeight > safeHeight)
					height = controlHeight;

				nextX += (control.width.Value + control.hSpacing);
				start.x = nextX;
			}

#if dbg
		Debug.Log(watch.ElapsedMilliseconds);
#endif
		}

		public override Layout Space(float pxl)
		{
			return new Layout { width = pxl };
		}
	}
}
