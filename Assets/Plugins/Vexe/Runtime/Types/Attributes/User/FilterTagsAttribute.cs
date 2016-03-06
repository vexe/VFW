using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Similar to FilterEnum - but works alongside TagsAttribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class FilterTagsAttribute : CompositeAttribute
	{
		public FilterTagsAttribute() : this(-1)
		{
		}

		public FilterTagsAttribute(int id) : base(id)
		{
		}
	}
}