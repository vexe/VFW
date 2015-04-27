using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Similar to Unity's RangeAttribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class iSliderAttribute : DrawnAttribute
	{
		public readonly int left, right;

		public iSliderAttribute(int left, int right)
		{
			this.left  = left;
			this.right = right;
		}
	}

	/// <summary>
	/// Similar to Unity's RangeAttribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class fSliderAttribute : DrawnAttribute
	{
		public readonly float left, right;

		public fSliderAttribute(float left, float right)
		{
			this.left  = left;
			this.right = right;
		}
	}
}