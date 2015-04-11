using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Annotate fields and properties of any type with this attribute to make it possible
	/// to assign values from a source gameObject and target component
	/// Note: The source, target and member info are saved (survive reloads) as long as the source gameObject is in the scene
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class AssignableAttribute : CompositeAttribute
	{
		public AssignableAttribute() : this(-1)
		{
		}

		public AssignableAttribute(int id) : base(id)
		{
		}
	}
}