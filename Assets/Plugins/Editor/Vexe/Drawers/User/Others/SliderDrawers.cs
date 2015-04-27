using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
	public class iSliderDrawer : AttributeDrawer<int, iSliderAttribute>
	{
		public override void OnGUI()
		{
			memberValue = gui.IntSlider(displayText, memberValue, attribute.left, attribute.right);
		}
	}

	public class fSliderDrawer : AttributeDrawer<float, fSliderAttribute>
	{
		public override void OnGUI()
		{
			memberValue = gui.FloatSlider(displayText, memberValue, attribute.left, attribute.right);
		}
	}
}