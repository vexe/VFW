using Vexe.Runtime.Types;

namespace VFWExamples
{
	public class SlidersExample : BaseBehaviour
	{
		[fSlider(-1.5f, 10.5f)] public float floatSlider;
		[iSlider(3, 15)] public int intSlider;
	}
}