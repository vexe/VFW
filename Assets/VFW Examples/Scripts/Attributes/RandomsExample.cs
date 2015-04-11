namespace Vexe.Runtime.Types.Examples
{
	public class RandomsExample : BetterBehaviour
	{
		[Comment("This is a random field between 10.5 and 25.75"), Rand(10.5f, 25.75f)]
		public float randomFloat;

		[Comment("This is a random property between 10 and 100"), Rand(10, 100)]
		public int RandomIntProperty { get; set; }
	}
}