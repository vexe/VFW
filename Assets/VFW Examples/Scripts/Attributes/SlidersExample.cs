using Vexe.Runtime.Types;
namespace VFWExamples
{
	
	public class SlidersExample : BetterBehaviour
	{
		[fSlider(-1.5f, 10.5f)]
		public float floatSlider;

		[iSlider(3, 15)]
		public int IntSlider { get; set; }
	}
}