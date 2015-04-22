using Vexe.Runtime.Types;

namespace VFWExamples
{ 
	
	public class StaticsExample : BetterBehaviour
	{
		public static int publicField;
		[Serialize] private static float PrivateProperty { get; set; }

		[Serialize, Hide] private static string _string;
		[Show] protected static string SomeString
		{
			get { return _string; }
			set { _string = value; }
		}
	}
}
