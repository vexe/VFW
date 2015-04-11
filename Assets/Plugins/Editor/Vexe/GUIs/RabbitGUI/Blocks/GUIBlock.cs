using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vexe.Editor.GUIs
{
	public abstract class GUIBlock : GUIControl, IDisposable
	{
		public List<GUIControl> controls;
		public Action onDisposed;

		public int nBlocksWithin { get; private set; }

		public GUIBlock()
		{
			controls = new List<GUIControl>();
		}

		public void AddBlock(GUIBlock block)
		{
			nBlocksWithin++;
			controls.Add(block);
		}

		public void Dispose()
		{
			onDisposed();
		}

		protected float GetWidth(Layout option, Rect rect)
		{
			if (option != null && option.width.HasValue)
				return option.width.Value;

			return width.HasValue ? width.Value : rect.width;
		}

		public override void ResetDimensions()
		{
			base.ResetDimensions();
			for (int i = 0; i < controls.Count; i++)
				controls[i].ResetDimensions();
		}

		public abstract void Layout(Rect start);
		public abstract Layout Space(float pxl);
	}
}