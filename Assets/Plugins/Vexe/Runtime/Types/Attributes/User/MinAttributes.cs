using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Annotate a float with this attribute to constrain it to a min value
	/// so it won't go below that value
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class fMinAttribute : ConstrainValueAttribute
	{
		public readonly float min;

		public fMinAttribute(int id, float min) : base(id)
		{
			this.min = min;
		}

		public fMinAttribute(float min) : this(-1, min)
		{
		}
	}

	/// <summary>
	/// Annotate an int with this attribute to constrain it to a min value
	/// so it won't go below that value
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class iMinAttribute : ConstrainValueAttribute
	{
		public readonly int min;

		public iMinAttribute(int id, int min) : base(id)
		{
			this.min = min;
		}

		public iMinAttribute(int min) : this(-1, min)
		{
		}
	}
}