using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Similar to Unity's RangeAttribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class IntSliderAttribute : DrawnAttribute
	{
		public readonly int left, right;

		public IntSliderAttribute(int left, int right)
		{
			this.left  = left;
			this.right = right;
		}
	}

	/// <summary>
	/// Similar to Unity's RangeAttribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class FloatSliderAttribute : DrawnAttribute
	{
		public readonly float left, right;

		public FloatSliderAttribute(float left, float right)
		{
			this.left  = left;
			this.right = right;
		}
	}
}