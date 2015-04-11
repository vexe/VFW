using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Annotate an int or float with this attribute to constrain it between a min and max values
	/// i.e. it won't go below min, nor above max
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class NumericClampAttribute : ConstrainValueAttribute
	{
		public readonly int intMin;
		public readonly int intMax;
		public readonly float floatMax;
		public readonly float floatMin;

		public NumericClampAttribute(int id, int min, int max) : base(id)
		{
			intMin = min;
			intMax = max;
		}

		public NumericClampAttribute(int id, float min, float max) : base(id)
		{
			floatMin = min;
			floatMax = max;
		}

		public NumericClampAttribute(int min, int max) : this(-1, min, max)
		{
		}

		public NumericClampAttribute(float min, float max) : this(-1, min, max)
		{
		}
	}
}