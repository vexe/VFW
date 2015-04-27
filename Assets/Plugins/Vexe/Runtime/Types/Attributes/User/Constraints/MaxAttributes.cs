using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Annotate a float with this attribute to constrain it to a max value
	/// so it won't go above that value
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class fMaxAttribute : ConstrainValueAttribute
	{
		public readonly float max;

		public fMaxAttribute(int id, float max) : base(id)
		{
			this.max = max;
		}

		public fMaxAttribute(float max) : this(-1, max)
		{
		}
	}

    /// <summary>
	/// Annotate an int with this attribute to constrain it to a max value
	/// so it won't go above that value
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class iMaxAttribute : ConstrainValueAttribute
	{
		public readonly int max;

		public iMaxAttribute(int id, int max) : base(id)
		{
			this.max = max;
		}

		public iMaxAttribute(int max) : this(-1, max)
		{
		}
	}
}