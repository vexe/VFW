using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Annotate a UnityEngine.Object with this attribute to make it draggable
	/// i.e. the field itself could be dragged and dropped somewhere else
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class DraggableAttribute : CompositeAttribute
	{
		public DraggableAttribute() : this(-1)
		{
		}

		public DraggableAttribute(int id) : base(id)
		{
		}
	}
}