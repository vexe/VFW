using System.Collections.Generic;
using UnityEngine;

namespace Vexe.Editor
{
	public class Layout
	{
		public float? height      { get; set; }
		public float? width       { get; set; }
		public bool? expandHeight { get; set; }
		public bool? expandWidth  { get; set; }
		public float? minHeight   { get; set; }
		public float? minWidth    { get; set; }
		public float? maxHeight   { get; set; }
		public float? maxWidth    { get; set; }

		public static List<GUILayoutOption> sharedGLOptions;
		public static readonly Layout None;

		public static Layout sWidth(float w)
		{
			return new Layout { width = w };
		}

		public static Layout sHeight(float w)
		{
			return new Layout { height = w };
		}

		public static Layout sExpandWidth(bool exp = true)
		{
			return new Layout { expandWidth = exp };
		}

		public static Layout sExpandHeight(bool exp = true)
		{
			return new Layout { expandHeight = exp };
		}

		public Layout Width(float w)
		{
			width = w;
			return this;
		}

		public Layout Height(float h)
		{
			height = h;
			return this;
		}

		public Layout ExpandWidth(bool exp = true)
		{
			expandWidth = exp;
			return this;
		}

		public Layout ExpandHeight(bool exp = true)
		{
			expandHeight = exp;
			return this;
		}

		static Layout()
		{
			sharedGLOptions = new List<GUILayoutOption>();
			None = new Layout();
		}

		public static implicit operator GUILayoutOption[](Layout option)
		{
			return option == null ? null : option.ToGLOptions();
		}
	}

	public static class LayoutOptionExtensions
	{
		public static GUILayoutOption[] ToGLOptions(this Layout option)
		{
			if (option == null) return null;
			if (option.width.HasValue)
				Layout.sharedGLOptions.Add(GUILayout.Width(option.width.Value));
			if (option.height.HasValue)
				Layout.sharedGLOptions.Add(GUILayout.Height(option.height.Value));
			if (option.minHeight.HasValue)
				Layout.sharedGLOptions.Add(GUILayout.MinHeight(option.minHeight.Value));
			if (option.minWidth.HasValue)
				Layout.sharedGLOptions.Add(GUILayout.MinWidth(option.minWidth.Value));
			if (option.maxHeight.HasValue)
				Layout.sharedGLOptions.Add(GUILayout.MaxHeight(option.maxHeight.Value));
			if (option.maxWidth.HasValue)
				Layout.sharedGLOptions.Add(GUILayout.MaxWidth(option.maxWidth.Value));
			if (option.expandHeight.HasValue)
				Layout.sharedGLOptions.Add(GUILayout.ExpandHeight(option.expandHeight.Value));
			if (option.expandWidth.HasValue)
				Layout.sharedGLOptions.Add(GUILayout.ExpandWidth(option.expandWidth.Value));
			return Layout.sharedGLOptions.ToArray(); // TODO: Garbage!
		}
	}
}