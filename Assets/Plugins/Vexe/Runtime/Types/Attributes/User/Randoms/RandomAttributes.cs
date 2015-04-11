namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Decorate a float/int with this attribute to randomize its value between min and max
	/// </summary>
	public class RandAttribute : CompositeAttribute
	{
		public readonly float floatMin;
		public readonly float floatMax;

		public readonly int intMin;
		public readonly int intMax;

		public RandAttribute(int id, float min, float max) : base(id)
		{
			floatMin = min;
			floatMax = max;
		}

		public RandAttribute(float min, float max) : this(-1, min, max)
		{
		}

		public RandAttribute(int id, int min, int max) : base(id)
		{
			intMin = min;
			intMax = max;
		}

		public RandAttribute(int min, int max) : this(-1, min, max)
		{
		}
	}
}