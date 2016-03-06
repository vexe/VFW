using Vexe.Runtime.Types;

namespace VFWExamples
{
	public class DecorationsExample : BaseBehaviour
	{
		[Comment("This is a string property!!!!")]
		public string StrProperty { get; set; }

		[Whitespace(0, Top = 100f), Comment(1, "I'm an int field, and I'm 100 pxs down!")]
		public int intField;

       [Comment("This is a help text", helpButton:true)]
        public string StrProperty2 { get; set; }
	}
}