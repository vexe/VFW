namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Decorate an int with this attribute to randomize its value between min and max
	/// </summary>
	public class iRandAttribute : CompositeAttribute
	{
		public readonly int min;
		public readonly int max;

		public iRandAttribute(int id, int min, int max) : base(id)
		{
			this.min = min;
			this.max = max;
		}

		public iRandAttribute(int min, int max) : this(-1, min, max)
		{
		}
	}
	/// <summary>
	/// Decorate a float with this attribute to randomize its value between min and max
	/// </summary>
	public class fRandAttribute : CompositeAttribute
	{
		public readonly float min;
		public readonly float max;

		public fRandAttribute(int id, float min, float max) : base(id)
		{
			this.min = min;
			this.max = max;
		}

		public fRandAttribute(float min, float max) : this(-1, min, max)
		{
		}
	}
}