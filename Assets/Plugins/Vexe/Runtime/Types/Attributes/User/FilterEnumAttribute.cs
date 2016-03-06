using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Annotate an enum with this attribute to display a text field on its left to 
	/// filter its values for quick selection
	/// Tip: Can be used with SelectEnum to have both a filter and a selection button for the enum
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class FilterEnumAttribute : CompositeAttribute
	{
		public FilterEnumAttribute() : this(-1)
		{
		}

		public FilterEnumAttribute(int id) : base(id)
		{
		}
	}
}