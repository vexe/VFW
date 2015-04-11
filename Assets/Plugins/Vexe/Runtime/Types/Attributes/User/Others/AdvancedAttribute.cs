using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// A general purpose attributes used by some drawers to help configure them
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class AdvancedAttribute : CompositeAttribute
	{
		public AdvancedAttribute() : this(-1)
		{
		}

		public AdvancedAttribute(int id) : base(id)
		{
		}
	}
}