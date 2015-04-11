using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Annotate strings with this to make it possible to assign to objects names and files/folders and assign their paths
	/// You can hold Ctrl and click the middle mouse button while hovering over the text field to display a selection window
	/// of all the gameObjects in the scene to pick one
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class PathAttribute : CompositeAttribute
	{
		/// <summary>
		/// Use full path name for scene objects and assets?
		/// </summary>
		public bool UseFullPath { get; set; }

		/// <summary>
		/// If false, the asset path is taken relative to Application.dataPath
		/// </summary>
		public bool AbsoluteAssetPath { get; set; }

		public PathAttribute(int id, bool useFullPath) : base(id)
		{
			UseFullPath = useFullPath;
		}

		public PathAttribute(bool useFullPath) : this(-1, useFullPath)
		{
		}

		public PathAttribute(int id) : this(id, true)
		{
		}

		public PathAttribute() : this(-1)
		{
		}
	}
}