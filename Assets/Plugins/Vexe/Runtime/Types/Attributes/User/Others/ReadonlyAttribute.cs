using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Fields marked-up with this attribute cannot be modified from the inspector
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class ReadonlyAttribute : CompositeAttribute
	{
		/// <summary>
		/// True to make the field modifiable from the inspector at edit time but not runtime
		/// </summary>
		public bool AssignAtEditTime { get; set; }

		public ReadonlyAttribute()
		{
		}

		public ReadonlyAttribute(int id) : base(id)
		{
		}
	}
}