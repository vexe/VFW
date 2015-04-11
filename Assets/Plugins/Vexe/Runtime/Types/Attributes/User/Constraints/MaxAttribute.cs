using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Annotate an int or float with this attribute to constrain it to a max value
	/// so it won't go above that value
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class MaxAttribute : ConstrainValueAttribute
	{
		public readonly float floatMax;
		public readonly int intMax;

		public MaxAttribute(int id, int max) : base(id)
		{
			intMax = max;
		}

		public MaxAttribute(int id, float max) : base(id)
		{
			floatMax = max;
		}

		public MaxAttribute(int max) : this(-1, max)
		{
		}

		public MaxAttribute(float max) : this(-1, max)
		{
		}
	}
}