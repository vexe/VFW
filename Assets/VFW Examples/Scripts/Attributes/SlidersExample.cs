using Vexe.Runtime.Types;
namespace VFWExamples
{
	
	public class SlidersExample : BetterBehaviour
	{
		[FloatSlider(0, 10)]
		public float floatSlider;

		[IntSlider(3, 15)]
		public int IntSlider { get; set; }
	}
}