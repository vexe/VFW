using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Annotate a member with this to specify its display order within its category
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
	public class DisplayOrderAttribute : Attribute
	{
		public readonly float displayOrder;

		public DisplayOrderAttribute() : this(-1)
		{
		}

		public DisplayOrderAttribute(float displayOrder)
		{
			this.displayOrder = displayOrder;
		}
	}
}
