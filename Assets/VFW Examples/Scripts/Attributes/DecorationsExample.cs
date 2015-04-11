namespace Vexe.Runtime.Types.Examples
{
	public class DecorationsExample : BetterBehaviour
	{
		[Comment("This is a string property!!!!")]
		public string StrProperty { get; set; }

		[Whitespace(0, Top = 100f), Comment(1, "I'm an int field, and I'm 100 pxs down!")]
		public int intField;
	}
}