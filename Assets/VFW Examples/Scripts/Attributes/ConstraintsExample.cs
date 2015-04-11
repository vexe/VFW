namespace Vexe.Runtime.Types.Examples
{
	
	public class ConstraintsExample : BetterBehaviour
	{
		[Comment("Example value: 1.0.0.127"), IP]
		public string ip;

		[Comment("Can't go further than 7.5"), Max(7.5f)]
		public float MaxFloatTo7Point5 { get; set; }

		[Comment("Can't go further than 100"), Max(100)]
		public int maxIntTo100;

		[Comment("Can't go below 0"), Min(0)]
		public int MinIntTo0 { get; set; }

		[Comment("Can't go below 1.5"), Min(1.5f)]
		public float minFloatTo1Point5;

		[Comment("Length must be between 5 and 15"), StringClamp(5, 15)]
		public string clampedStringTo10;

		[Comment("Value must be between 1.5 and 10.75"), NumericClamp(1.5f, 10.75f)]
		public float clampedFloat;

		[Comment("Value must be between 3 and 10"), NumericClamp(3, 10)]
		public int ClampedInt { get; set; }

		[Comment("Value must start with `name:`"), Regex("^name:")]
		public string mustStartWithName;
	}
}