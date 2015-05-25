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

        /// <summary>
        /// Return Auto layout (null) to tell the layout system to figure out the control dimensions itself
        /// </summary>
        public static readonly Layout Auto = null;

		public static readonly Layout None;

		public static List<GUILayoutOption> SharedGLOptions;

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
            Debug.LogWarning("ExpandWidth is not implemented, it won't do anything. Returning Auto layout");
            return Auto;
		}

		public static Layout sExpandHeight(bool exp = true)
		{
            Debug.LogWarning("ExpandHeight is not implemented, it won't do anything. Returning Auto layout");
            return Auto;
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
            Debug.LogWarning("ExpandWidth is not implemented, it won't do anything. Returning Auto layout");
			return Auto;
		}

		public Layout ExpandHeight(bool exp = true)
		{
            Debug.LogWarning("ExpandHeight is not implemented, it won't do anything. Returning Auto layout");
            return Auto;
		}

		static Layout()
		{
			SharedGLOptions = new List<GUILayoutOption>();
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
				Layout.SharedGLOptions.Add(GUILayout.Width(option.width.Value));
			if (option.height.HasValue)
				Layout.SharedGLOptions.Add(GUILayout.Height(option.height.Value));
			if (option.minHeight.HasValue)
				Layout.SharedGLOptions.Add(GUILayout.MinHeight(option.minHeight.Value));
			if (option.minWidth.HasValue)
				Layout.SharedGLOptions.Add(GUILayout.MinWidth(option.minWidth.Value));
			if (option.maxHeight.HasValue)
				Layout.SharedGLOptions.Add(GUILayout.MaxHeight(option.maxHeight.Value));
			if (option.maxWidth.HasValue)
				Layout.SharedGLOptions.Add(GUILayout.MaxWidth(option.maxWidth.Value));
			if (option.expandHeight.HasValue)
				Layout.SharedGLOptions.Add(GUILayout.ExpandHeight(option.expandHeight.Value));
			if (option.expandWidth.HasValue)
				Layout.SharedGLOptions.Add(GUILayout.ExpandWidth(option.expandWidth.Value));
			return Layout.SharedGLOptions.ToArray(); // TODO: Garbage!
		}
	}
}