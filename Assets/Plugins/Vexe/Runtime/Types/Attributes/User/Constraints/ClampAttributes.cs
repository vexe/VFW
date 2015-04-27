using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Annotate a string with this attribute to clamp its length between a min and max values
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class sClampAttribute : RegexAttribute
	{
		public readonly int min;
		public readonly int max;

		public sClampAttribute(int min, int max)
			: this(-1, min, max)
		{
		}

		public sClampAttribute(int id, int min, int max)
			: base(id, "^.{" + min + "," + max + "}$")
		{
			this.min = min;
			this.max = max;
		}
	}

	/// <summary>
	/// Annotate a float with this attribute to constrain it between a min and max values
	/// i.e. it won't go below min, nor above max
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class fClampAttribute : ConstrainValueAttribute
	{
		public readonly float max;
		public readonly float min;

		public fClampAttribute(int id, float min, float max) : base(id)
		{
			this.min = min;
			this.max = max;
		}

		public fClampAttribute(float min, float max) : this(-1, min, max)
		{
		}
	}
	/// <summary>
	/// Annotate an int with this attribute to constrain it between a min and max values
	/// i.e. it won't go below min, nor above max
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
	public class iClampAttribute : ConstrainValueAttribute
	{
		public readonly int min;
		public readonly int max;

		public iClampAttribute(int id, int min, int max) : base(id)
		{
			this.min = min;
			this.max = max;
		}

		public iClampAttribute(int min, int max) : this(-1, min, max)
		{
		}
	}
}